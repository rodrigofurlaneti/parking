namespace Parking.Application.Features.Sale.RegisterSale;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using DomainSale = Parking.Domain.Entities.Sale;

internal sealed class RegisterSaleCommandHandler : ICommandHandler<RegisterSaleCommand, SaleDto>
{
    private const decimal ToleranceAmount = 0.01m;

    private readonly IVehicleExitRepository _vehicleExitRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ISalePaymentRepository _salePaymentRepository;
    private readonly ICashRegisterRepository _cashRegisterRepository;
    private readonly ICashMovementRepository _cashMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterSaleCommandHandler(
        IVehicleExitRepository vehicleExitRepository,
        ISaleRepository saleRepository,
        ISalePaymentRepository salePaymentRepository,
        ICashRegisterRepository cashRegisterRepository,
        ICashMovementRepository cashMovementRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleExitRepository = vehicleExitRepository;
        _saleRepository = saleRepository;
        _salePaymentRepository = salePaymentRepository;
        _cashRegisterRepository = cashRegisterRepository;
        _cashMovementRepository = cashMovementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SaleDto>> Handle(RegisterSaleCommand request, CancellationToken cancellationToken)
    {
        var vehicleExit = await _vehicleExitRepository.GetByIdAsync(request.VehicleExitId, cancellationToken);
        if (vehicleExit is null)
            return Result.Failure<SaleDto>(new Error("VehicleExit.NotFound", "Vehicle exit not found."));

        var alreadyExists = await _saleRepository.ExistsActiveByVehicleExitAsync(request.VehicleExitId, cancellationToken);
        if (alreadyExists)
            return Result.Failure<SaleDto>(new Error("Sale.AlreadyExists", "A sale already exists for this vehicle exit."));

        var cashRegister = await _cashRegisterRepository.GetByIdAsync(request.CashRegisterId, cancellationToken);
        if (cashRegister is null)
            return Result.Failure<SaleDto>(new Error("CashRegister.NotFound", "Cash register not found."));

        if (cashRegister.Status != 0)
            return Result.Failure<SaleDto>(new Error("CashRegister.Closed", "Cash register is not open."));

        var paymentsTotal = request.Payments.Sum(p => p.Amount);
        if (Math.Abs(paymentsTotal - vehicleExit.TotalAmount) > ToleranceAmount)
            return Result.Failure<SaleDto>(
                new Error("Sale.PaymentMismatch", "Sum of payments does not match the vehicle exit total amount."));

        var saleNumber = await _saleRepository.GetNextSaleNumberAsync(request.BranchId, cancellationToken);

        var saleResult = DomainSale.Create(
            request.BranchId,
            request.VehicleExitId,
            request.CashRegisterId,
            saleNumber,
            vehicleExit.TotalAmount);

        if (saleResult.IsFailure)
            return Result.Failure<SaleDto>(saleResult.Error);

        var sale = saleResult.Value;

        // The insert of Sale and the inserts of its dependent SalePayment/CashMovement rows are
        // split across two SaveChanges calls, because the SalePayment/CashMovement rows need the
        // DB-generated identity (sale.Id) from the first insert. Wrap both in a single ambient
        // database transaction so a failure in the second SaveChanges rolls back the first one too,
        // avoiding an orphaned Sale without payments/cash movement.
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _saleRepository.AddAsync(sale, cancellationToken);

            // Flush the insert so the DB-generated identity (sale.Id) is available
            // to build the SalePayment/CashMovement rows that reference it by FK value.
            await _unitOfWork.CommitAsync(cancellationToken);

            var paymentDtos = new List<SalePaymentDto>();
            foreach (var paymentInput in request.Payments)
            {
                var paymentResult = SalePayment.Create(sale.Id, paymentInput.PaymentMethod, paymentInput.Amount);
                if (paymentResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<SaleDto>(paymentResult.Error);
                }

                await _salePaymentRepository.AddAsync(paymentResult.Value, cancellationToken);
                paymentDtos.Add(new SalePaymentDto(
                    paymentResult.Value.Id,
                    paymentResult.Value.PaymentMethod,
                    paymentResult.Value.Amount));
            }

            var movementResult = CashMovement.Create(
                request.CashRegisterId,
                1,
                vehicleExit.TotalAmount,
                $"Venda #{saleNumber}");

            if (movementResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<SaleDto>(movementResult.Error);
            }

            await _cashMovementRepository.AddAsync(movementResult.Value, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(new SaleDto(
                sale.Id,
                sale.BranchId,
                sale.VehicleExitId,
                sale.SaleNumber,
                sale.TotalAmount,
                sale.SaleDate,
                sale.IsActive,
                paymentDtos));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
