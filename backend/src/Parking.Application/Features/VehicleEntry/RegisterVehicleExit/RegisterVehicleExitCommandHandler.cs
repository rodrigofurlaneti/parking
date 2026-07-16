namespace Parking.Application.Features.VehicleEntry.RegisterVehicleExit;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class RegisterVehicleExitCommandHandler : ICommandHandler<RegisterVehicleExitCommand, VehicleExitDto>
{
    private readonly IVehicleEntryRepository _vehicleEntryRepository;
    private readonly IVehicleExitRepository _vehicleExitRepository;
    private readonly IParkingSpaceRepository _parkingSpaceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITariffRepository _tariffRepository;
    private readonly IAgreementCustomerContractRepository _agreementCustomerContractRepository;
    private readonly IAgreementMerchantRepository _agreementMerchantRepository;
    private readonly IMonthlyCustomerContractRepository _monthlyCustomerContractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterVehicleExitCommandHandler(
        IVehicleEntryRepository vehicleEntryRepository,
        IVehicleExitRepository vehicleExitRepository,
        IParkingSpaceRepository parkingSpaceRepository,
        ICustomerRepository customerRepository,
        ITariffRepository tariffRepository,
        IAgreementCustomerContractRepository agreementCustomerContractRepository,
        IAgreementMerchantRepository agreementMerchantRepository,
        IMonthlyCustomerContractRepository monthlyCustomerContractRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleEntryRepository = vehicleEntryRepository;
        _vehicleExitRepository = vehicleExitRepository;
        _parkingSpaceRepository = parkingSpaceRepository;
        _customerRepository = customerRepository;
        _tariffRepository = tariffRepository;
        _agreementCustomerContractRepository = agreementCustomerContractRepository;
        _agreementMerchantRepository = agreementMerchantRepository;
        _monthlyCustomerContractRepository = monthlyCustomerContractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VehicleExitDto>> Handle(RegisterVehicleExitCommand request, CancellationToken cancellationToken)
    {
        var vehicleEntry = await _vehicleEntryRepository.GetByIdAsync(request.VehicleEntryId, cancellationToken);
        if (vehicleEntry is null)
            return Result.Failure<VehicleExitDto>(new Error("VehicleEntry.NotFound", "Vehicle entry not found."));

        if (vehicleEntry.Status != 0)
            return Result.Failure<VehicleExitDto>(
                new Error("VehicleEntry.AlreadyExited", "Vehicle already exited."));

        var customer = await _customerRepository.GetByIdAsync(vehicleEntry.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<VehicleExitDto>(new Error("Customer.NotFound", "Customer not found."));

        var durationMinutes = vehicleEntry.GetDurationMinutes();
        var parkingMode = customer.CustomerType;

        var amountResult = await CalculateTotalAmountAsync(vehicleEntry, customer, durationMinutes, cancellationToken);
        if (amountResult.IsFailure)
            return Result.Failure<VehicleExitDto>(amountResult.Error);

        var vehicleExitResult = VehicleExit.Create(
            request.VehicleEntryId,
            durationMinutes,
            amountResult.Value,
            parkingMode);

        if (vehicleExitResult.IsFailure)
            return Result.Failure<VehicleExitDto>(vehicleExitResult.Error);

        vehicleEntry.MarkAsExited();

        var parkingSpace = await _parkingSpaceRepository.GetByIdAsync(vehicleEntry.ParkingSpaceId, cancellationToken);
        if (parkingSpace is not null)
        {
            parkingSpace.MarkAsAvailable();
            await _parkingSpaceRepository.UpdateAsync(parkingSpace, cancellationToken);
        }

        await _vehicleExitRepository.AddAsync(vehicleExitResult.Value, cancellationToken);
        await _vehicleEntryRepository.UpdateAsync(vehicleEntry, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new VehicleExitDto(
            vehicleExitResult.Value.Id,
            vehicleExitResult.Value.VehicleEntryId,
            vehicleExitResult.Value.ExitTime,
            vehicleExitResult.Value.DurationMinutes,
            vehicleExitResult.Value.TotalAmount,
            vehicleExitResult.Value.ParkingMode,
            vehicleExitResult.Value.IsActive));
    }

    // Regras de cobranca por CustomerType:
    // 1 (Rotativo)  - cobra pela tarifa ativa da filial. Sem tarifa cadastrada, falha.
    // 2 (Convenio)  - calcula o valor base com a mesma tarifa do Rotativo e aplica o desconto do
    //                 AgreementMerchant vinculado via contrato ativo. Sem contrato/merchant
    //                 validos, cai para o comportamento de Rotativo (cobranca cheia) em vez de
    //                 falhar, pois o cliente esta de fato ocupando uma vaga e precisa pagar algo.
    // 3 (Mensalista) - com contrato mensal ativo na data da saida, a saida e gratuita (o cliente
    //                 ja paga a mensalidade fixa separadamente). Sem contrato valido, cai para o
    //                 comportamento de Rotativo (cobranca cheia pela tarifa), pelo mesmo motivo
    //                 acima: o veiculo usou a vaga sem cobertura contratual.
    private async Task<Result<decimal>> CalculateTotalAmountAsync(
        VehicleEntry vehicleEntry,
        Customer customer,
        int durationMinutes,
        CancellationToken cancellationToken)
    {
        if (customer.CustomerType == 3)
        {
            var monthlyContracts = await _monthlyCustomerContractRepository.GetActiveByCustomerAsync(
                customer.Id, cancellationToken);
            var validMonthlyContract = monthlyContracts.FirstOrDefault(c => c.IsValidOn(DateTime.UtcNow));

            if (validMonthlyContract is not null)
                return Result.Success(0m);
        }

        var tariff = await _tariffRepository.GetActiveByBranchAsync(vehicleEntry.BranchId, cancellationToken);
        if (tariff is null)
            return Result.Failure<decimal>(
                new Error("Tariff.NotConfigured", "No active tariff configured for this branch."));

        var baseAmount = tariff.CalculateAmount(durationMinutes);

        if (customer.CustomerType == 2)
        {
            var agreementContracts = await _agreementCustomerContractRepository.GetActiveByCustomerAsync(
                customer.Id, cancellationToken);
            var validAgreementContract = agreementContracts.FirstOrDefault(c => c.IsValidOn(DateTime.UtcNow));

            if (validAgreementContract is not null)
            {
                var merchant = await _agreementMerchantRepository.GetByIdAsync(
                    validAgreementContract.AgreementMerchantId, cancellationToken);

                if (merchant is not null)
                {
                    var discountedAmount = baseAmount * (1 - (merchant.DiscountPercentage / 100m));
                    return Result.Success(discountedAmount);
                }
            }
        }

        return Result.Success(baseAmount);
    }
}
