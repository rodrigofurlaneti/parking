namespace Parking.Application.Features.ParkingSpace.GetParkingSpaceOccupancy;

using Parking.Application.Abstractions.Messaging;

public sealed record GetParkingSpaceOccupancyQuery(long BranchId) : IQuery<ParkingSpaceOccupancyDto>;

public sealed record ParkingSpaceOccupancyDto(
    long BranchId,
    int TotalSpaces,
    int OccupiedSpaces,
    int AvailableSpaces,
    decimal OccupancyRate);
