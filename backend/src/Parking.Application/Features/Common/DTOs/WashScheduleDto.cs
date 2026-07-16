namespace Parking.Application.Features.Common.DTOs;

public sealed record WashScheduleDto(
    long Id,
    long BranchId,
    long VehicleEntryId,
    DateTime ScheduledTime,
    DateTime? ActualStartTime,
    DateTime? ActualEndTime,
    long EmployeeId,
    int Status,
    bool IsActive);
