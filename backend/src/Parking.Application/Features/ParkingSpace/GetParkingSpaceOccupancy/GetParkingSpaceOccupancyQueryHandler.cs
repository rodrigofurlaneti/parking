namespace Parking.Application.Features.ParkingSpace.GetParkingSpaceOccupancy;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetParkingSpaceOccupancyQueryHandler : IQueryHandler<GetParkingSpaceOccupancyQuery, ParkingSpaceOccupancyDto>
{
    private readonly IParkingSpaceRepository _parkingSpaceRepository;

    public GetParkingSpaceOccupancyQueryHandler(IParkingSpaceRepository parkingSpaceRepository)
    {
        _parkingSpaceRepository = parkingSpaceRepository;
    }

    public async Task<Result<ParkingSpaceOccupancyDto>> Handle(GetParkingSpaceOccupancyQuery request, CancellationToken cancellationToken)
    {
        var spaces = await _parkingSpaceRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);
        if (spaces.Count == 0)
            return Result.Failure<ParkingSpaceOccupancyDto>(
                new Error("ParkingSpace.NotFound", "No parking spaces found for this branch."));

        var occupiedCount = await _parkingSpaceRepository.GetOccupiedCountAsync(request.BranchId, cancellationToken);
        var availableCount = spaces.Count - occupiedCount;
        var occupancyRate = (decimal)occupiedCount / spaces.Count * 100;

        return Result.Success(new ParkingSpaceOccupancyDto(
            request.BranchId,
            spaces.Count,
            occupiedCount,
            availableCount,
            occupancyRate));
    }
}
