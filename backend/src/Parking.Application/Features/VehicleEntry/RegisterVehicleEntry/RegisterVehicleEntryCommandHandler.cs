namespace Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class RegisterVehicleEntryCommandHandler : ICommandHandler<RegisterVehicleEntryCommand, VehicleEntryDto>
{
    private readonly IVehicleEntryRepository _vehicleEntryRepository;
    private readonly IParkingSpaceRepository _parkingSpaceRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMonthlyCustomerContractRepository _monthlyCustomerContractRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterVehicleEntryCommandHandler(
        IVehicleEntryRepository vehicleEntryRepository,
        IParkingSpaceRepository parkingSpaceRepository,
        ICustomerRepository customerRepository,
        IMonthlyCustomerContractRepository monthlyCustomerContractRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleEntryRepository = vehicleEntryRepository;
        _parkingSpaceRepository = parkingSpaceRepository;
        _customerRepository = customerRepository;
        _monthlyCustomerContractRepository = monthlyCustomerContractRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VehicleEntryDto>> Handle(RegisterVehicleEntryCommand request, CancellationToken cancellationToken)
    {
        var parkingSpace = await _parkingSpaceRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);
        if (parkingSpace is null)
            return Result.Failure<VehicleEntryDto>(new Error("ParkingSpace.NotFound", "Parking space not found."));

        if (parkingSpace.Status != 0)
            return Result.Failure<VehicleEntryDto>(
                new Error("ParkingSpace.NotAvailable", "Parking space is not available."));

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<VehicleEntryDto>(new Error("Customer.NotFound", "Customer not found."));

        if (customer.CustomerType == 3)
        {
            var activeContracts = await _monthlyCustomerContractRepository.GetActiveByCustomerAsync(
                request.CustomerId, cancellationToken);

            var validContract = activeContracts.FirstOrDefault(c => c.IsValidOn(DateTime.UtcNow));
            if (validContract is null)
                return Result.Failure<VehicleEntryDto>(
                    new Error("MonthlyContract.Invalid", "No active monthly contract for this customer."));

            var activeVehicleCount = await _vehicleRepository.CountActiveByCustomerAsync(
                request.CustomerId, cancellationToken);

            if (activeVehicleCount >= validContract.MaxVehicles)
            {
                var customerVehicles = await _vehicleRepository.GetByCustomerAsync(
                    request.CustomerId, cancellationToken);

                var plateAlreadyRegistered = customerVehicles.Any(v =>
                    string.Equals(v.LicensePlate, request.LicensePlate, StringComparison.OrdinalIgnoreCase));

                if (!plateAlreadyRegistered)
                    return Result.Failure<VehicleEntryDto>(
                        new Error("MonthlyContract.VehicleLimitExceeded", "Vehicle limit exceeded for this monthly customer."));
            }
        }

        var vehicleResult = VehicleEntry.Create(
            request.BranchId,
            request.ParkingSpaceId,
            request.CustomerId,
            request.LicensePlate,
            request.VehicleModel,
            request.VehicleColor);

        if (vehicleResult.IsFailure)
            return Result.Failure<VehicleEntryDto>(vehicleResult.Error);

        parkingSpace.MarkAsOccupied();

        await _vehicleEntryRepository.AddAsync(vehicleResult.Value, cancellationToken);
        await _parkingSpaceRepository.UpdateAsync(parkingSpace, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new VehicleEntryDto(
            vehicleResult.Value.Id,
            vehicleResult.Value.BranchId,
            vehicleResult.Value.ParkingSpaceId,
            vehicleResult.Value.CustomerId,
            vehicleResult.Value.LicensePlate,
            vehicleResult.Value.VehicleModel,
            vehicleResult.Value.VehicleColor,
            vehicleResult.Value.EntryTime,
            vehicleResult.Value.ExitTime,
            vehicleResult.Value.Status,
            vehicleResult.Value.IsActive));
    }
}
