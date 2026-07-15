namespace Parking.Application.Features.ParkingSpace.GetParkingSpaceDetails;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetParkingSpaceDetailsQueryHandler : IQueryHandler<GetParkingSpaceDetailsQuery, ParkingSpaceDetailsDto>
{
    private readonly IParkingSpaceRepository _parkingSpaceRepository;

    public GetParkingSpaceDetailsQueryHandler(IParkingSpaceRepository parkingSpaceRepository)
    {
        _parkingSpaceRepository = parkingSpaceRepository;
    }

    public async Task<Result<ParkingSpaceDetailsDto>> Handle(GetParkingSpaceDetailsQuery request, CancellationToken cancellationToken)
    {
        var parkingSpace = await _parkingSpaceRepository.GetByIdAsync(request.ParkingSpaceId, cancellationToken);
        if (parkingSpace is null)
            return Result.Failure<ParkingSpaceDetailsDto>(
                new Error("ParkingSpace.NotFound", "Parking space not found."));

        var statusDescription = parkingSpace.Status switch
        {
            0 => "Available",
            1 => "Occupied",
            2 => "Maintenance",
            _ => "Unknown"
        };

        return Result.Success(new ParkingSpaceDetailsDto(
            parkingSpace.Id,
            parkingSpace.BranchId,
            parkingSpace.SpaceNumber,
            parkingSpace.Type,
            parkingSpace.Status,
            statusDescription,
            parkingSpace.IsActive));
    }
}
