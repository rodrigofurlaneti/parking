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
    private readonly IUnitOfWork _unitOfWork;

    public RegisterVehicleExitCommandHandler(
        IVehicleEntryRepository vehicleEntryRepository,
        IVehicleExitRepository vehicleExitRepository,
        IParkingSpaceRepository parkingSpaceRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleEntryRepository = vehicleEntryRepository;
        _vehicleExitRepository = vehicleExitRepository;
        _parkingSpaceRepository = parkingSpaceRepository;
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

        var durationMinutes = vehicleEntry.GetDurationMinutes();

        var vehicleExitResult = VehicleExit.Create(
            request.VehicleEntryId,
            durationMinutes,
            request.TotalAmount,
            request.ParkingMode);

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
}
