namespace Parking.Application.Features.VehicleEntry.GetVehicleEntry;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetVehicleEntryQueryHandler : IQueryHandler<GetVehicleEntryQuery, VehicleEntryDto>
{
    private readonly IVehicleEntryRepository _vehicleEntryRepository;

    public GetVehicleEntryQueryHandler(IVehicleEntryRepository vehicleEntryRepository)
    {
        _vehicleEntryRepository = vehicleEntryRepository;
    }

    public async Task<Result<VehicleEntryDto>> Handle(GetVehicleEntryQuery request, CancellationToken cancellationToken)
    {
        var vehicleEntry = await _vehicleEntryRepository.GetByIdAsync(request.VehicleEntryId, cancellationToken);
        if (vehicleEntry is null)
            return Result.Failure<VehicleEntryDto>(new Error("VehicleEntry.NotFound", "Vehicle entry not found."));

        return Result.Success(new VehicleEntryDto(
            vehicleEntry.Id,
            vehicleEntry.BranchId,
            vehicleEntry.ParkingSpaceId,
            vehicleEntry.CustomerId,
            vehicleEntry.LicensePlate,
            vehicleEntry.VehicleModel,
            vehicleEntry.VehicleColor,
            vehicleEntry.EntryTime,
            vehicleEntry.ExitTime,
            vehicleEntry.Status,
            vehicleEntry.IsActive));
    }
}
