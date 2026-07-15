namespace Parking.Application.Features.Employee.GetEmployeeSchedule;

using Parking.Application.Abstractions.Messaging;

public sealed record GetEmployeeScheduleQuery(long EmployeeId) : IQuery<List<EmployeeScheduleDto>>;

public sealed record EmployeeScheduleDto(
    long Id,
    long EmployeeId,
    int DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsActive);
