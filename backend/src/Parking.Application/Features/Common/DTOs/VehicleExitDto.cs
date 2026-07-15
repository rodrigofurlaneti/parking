namespace Parking.Application.Features.Common.DTOs;

public sealed record VehicleExitDto(
    long Id,
    long VehicleEntryId,
    DateTime ExitTime,
    int DurationMinutes,
    decimal TotalAmount,
    int ParkingMode,
    bool IsActive);
