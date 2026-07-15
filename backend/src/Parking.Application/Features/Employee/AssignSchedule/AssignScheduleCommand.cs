namespace Parking.Application.Features.Employee.AssignSchedule;

using Parking.Application.Abstractions.Messaging;

public sealed record AssignScheduleCommand(
    long EmployeeId,
    int DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime) : ICommand<long>;
