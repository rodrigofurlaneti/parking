namespace Parking.Application.Features.Common.DTOs;

public sealed record ParkingSpaceDto(
    long Id,
    long BranchId,
    string SpaceNumber,
    int Type,
    int Status,
    bool IsActive);
