namespace Parking.Application.Features.Common.DTOs;

public sealed record VehicleEntryDto(
    long Id,
    long BranchId,
    long ParkingSpaceId,
    long CustomerId,
    string LicensePlate,
    string VehicleModel,
    string VehicleColor,
    DateTime EntryTime,
    DateTime? ExitTime,
    int Status,
    bool IsActive);
