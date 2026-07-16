namespace Parking.Application.Features.Vehicle.CreateVehicle;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainVehicle = Parking.Domain.Entities.Vehicle;

internal sealed class CreateVehicleCommandHandler : ICommandHandler<CreateVehicleCommand, VehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VehicleDto>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<VehicleDto>(new Error("Customer.NotFound", "Customer not found."));

        var vehicleResult = DomainVehicle.Create(
            request.CustomerId,
            request.LicensePlate,
            request.Model,
            request.Color);

        if (vehicleResult.IsFailure)
            return Result.Failure<VehicleDto>(vehicleResult.Error);

        var vehicle = vehicleResult.Value;
        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new VehicleDto(
            vehicle.Id,
            vehicle.CustomerId,
            vehicle.LicensePlate,
            vehicle.Model,
            vehicle.Color,
            vehicle.IsActive));
    }
}
