namespace Parking.Application.Features.ParkingSpace.GetParkingSpaceDetails;

using Parking.Application.Abstractions.Messaging;

public sealed record GetParkingSpaceDetailsQuery(long ParkingSpaceId) : IQuery<ParkingSpaceDetailsDto>;

public sealed record ParkingSpaceDetailsDto(
    long Id,
    long BranchId,
    string SpaceNumber,
    int Type,
    int Status,
    string StatusDescription,
    bool IsActive);
