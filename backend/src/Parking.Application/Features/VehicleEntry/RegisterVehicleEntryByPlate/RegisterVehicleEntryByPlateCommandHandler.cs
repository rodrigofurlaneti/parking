namespace Parking.Application.Features.VehicleEntry.RegisterVehicleEntryByPlate;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using DomainVehicle = Parking.Domain.Entities.Vehicle;

internal sealed class RegisterVehicleEntryByPlateCommandHandler
    : ICommandHandler<RegisterVehicleEntryByPlateCommand, VehicleEntryByPlateResultDto>
{
    private readonly IVehicleEntryRepository _vehicleEntryRepository;
    private readonly IParkingSpaceRepository _parkingSpaceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMonthlyCustomerContractRepository _monthlyCustomerContractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterVehicleEntryByPlateCommandHandler(
        IVehicleEntryRepository vehicleEntryRepository,
        IParkingSpaceRepository parkingSpaceRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        IMonthlyCustomerContractRepository monthlyCustomerContractRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleEntryRepository = vehicleEntryRepository;
        _parkingSpaceRepository = parkingSpaceRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _monthlyCustomerContractRepository = monthlyCustomerContractRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VehicleEntryByPlateResultDto>> Handle(
        RegisterVehicleEntryByPlateCommand request, CancellationToken cancellationToken)
    {
        var plate = request.LicensePlate.Trim().ToUpperInvariant();
        var model = request.VehicleModel?.Trim() ?? string.Empty;
        var color = request.VehicleColor?.Trim() ?? string.Empty;

        var parkingSpace = await _parkingSpaceRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);
        if (parkingSpace is null)
            return Result.Failure<VehicleEntryByPlateResultDto>(
                new Error("ParkingSpace.NotFound", "Parking space not found."));

        if (parkingSpace.Status != 0)
            return Result.Failure<VehicleEntryByPlateResultDto>(
                new Error("ParkingSpace.NotAvailable", "Parking space is not available."));

        var alreadyParked = await _vehicleEntryRepository.GetOpenByLicensePlateAsync(plate, cancellationToken);
        if (alreadyParked is not null)
            return Result.Failure<VehicleEntryByPlateResultDto>(
                new Error("VehicleEntry.AlreadyParked", "This license plate already has an open entry."));

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var vehicle = await _vehicleRepository.GetByLicensePlateAsync(plate, cancellationToken);
            Customer? customer;
            bool isNewCustomer;

            if (vehicle is not null)
            {
                customer = await _customerRepository.GetByIdAsync(vehicle.CustomerId, cancellationToken);
                if (customer is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<VehicleEntryByPlateResultDto>(
                        new Error("Customer.NotFound", "Customer linked to this vehicle was not found."));
                }

                isNewCustomer = false;
            }
            else
            {
                // Placa nunca vista: cadastra automaticamente um cliente "Avulso" (Rotativo),
                // sem exigir nenhum passo manual de cadastro antes da entrada.
                var document = $"AVULSO-{plate}";
                var customerResult = Customer.Create(
                    request.BranchId,
                    customerType: 1,
                    name: $"Cliente Avulso {plate}",
                    document: document,
                    phone: null,
                    email: null);

                if (customerResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<VehicleEntryByPlateResultDto>(customerResult.Error);
                }

                customer = customerResult.Value;
                await _customerRepository.AddAsync(customer, cancellationToken);
                // Flush intermediario (dentro da mesma transacao) so para obter o Id gerado do
                // cliente, necessario para criar o veiculo e a entrada em seguida.
                await _unitOfWork.CommitAsync(cancellationToken);

                var newVehicleResult = DomainVehicle.Create(customer.Id, plate, model, color);
                if (newVehicleResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<VehicleEntryByPlateResultDto>(newVehicleResult.Error);
                }

                vehicle = newVehicleResult.Value;
                await _vehicleRepository.AddAsync(vehicle, cancellationToken);
                isNewCustomer = true;
            }

            // Mesma regra de negocio de RegisterVehicleEntryCommandHandler: mensalista precisa de
            // contrato ativo e respeitar o limite de veiculos do contrato.
            if (customer.CustomerType == 3)
            {
                var activeContracts = await _monthlyCustomerContractRepository.GetActiveByCustomerAsync(
                    customer.Id, cancellationToken);
                var validContract = activeContracts.FirstOrDefault(c => c.IsValidOn(DateTime.UtcNow));

                if (validContract is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<VehicleEntryByPlateResultDto>(
                        new Error("MonthlyContract.Invalid", "No active monthly contract for this customer."));
                }

                var activeVehicleCount = await _vehicleRepository.CountActiveByCustomerAsync(
                    customer.Id, cancellationToken);

                if (activeVehicleCount >= validContract.MaxVehicles)
                {
                    var customerVehicles = await _vehicleRepository.GetByCustomerAsync(
                        customer.Id, cancellationToken);

                    var plateAlreadyRegistered = customerVehicles.Any(v =>
                        string.Equals(v.LicensePlate, plate, StringComparison.OrdinalIgnoreCase));

                    if (!plateAlreadyRegistered)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure<VehicleEntryByPlateResultDto>(
                            new Error("MonthlyContract.VehicleLimitExceeded", "Vehicle limit exceeded for this monthly customer."));
                    }
                }
            }

            var entryResult = VehicleEntry.Create(
                request.BranchId,
                request.ParkingSpaceId,
                customer.Id,
                plate,
                model,
                color);

            if (entryResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<VehicleEntryByPlateResultDto>(entryResult.Error);
            }

            parkingSpace.MarkAsOccupied();

            await _vehicleEntryRepository.AddAsync(entryResult.Value, cancellationToken);
            await _parkingSpaceRepository.UpdateAsync(parkingSpace, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var entry = entryResult.Value;
            return Result.Success(new VehicleEntryByPlateResultDto(
                entry.Id,
                entry.BranchId,
                entry.ParkingSpaceId,
                customer.Id,
                customer.Name,
                customer.CustomerType,
                isNewCustomer,
                entry.LicensePlate,
                entry.VehicleModel,
                entry.VehicleColor,
                entry.EntryTime,
                entry.ExitTime,
                entry.Status,
                entry.IsActive));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
