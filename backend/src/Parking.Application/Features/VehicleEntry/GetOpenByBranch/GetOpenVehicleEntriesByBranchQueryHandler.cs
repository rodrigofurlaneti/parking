namespace Parking.Application.Features.VehicleEntry.GetOpenByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetOpenVehicleEntriesByBranchQueryHandler : IQueryHandler<GetOpenVehicleEntriesByBranchQuery, List<VehicleEntryDto>>
{
    private readonly IVehicleEntryRepository _vehicleEntryRepository;

    public GetOpenVehicleEntriesByBranchQueryHandler(IVehicleEntryRepository vehicleEntryRepository)
    {
        _vehicleEntryRepository = vehicleEntryRepository;
    }

    public async Task<Result<List<VehicleEntryDto>>> Handle(GetOpenVehicleEntriesByBranchQuery request, CancellationToken cancellationToken)
    {
        var entries = await _vehicleEntryRepository.GetParkedByBranchAsync(request.BranchId, cancellationToken);

        var dtos = entries.Select(x => new VehicleEntryDto(
            x.Id,
            x.BranchId,
            x.ParkingSpaceId,
            x.CustomerId,
            x.LicensePlate,
            x.VehicleModel,
            x.VehicleColor,
            x.EntryTime,
            x.ExitTime,
            x.Status,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
