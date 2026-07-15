namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class EmployeeSchedule : Entity
{
    public long EmployeeId { get; private set; }
    public int DayOfWeek { get; private set; } // 0-6 (Monday-Sunday)
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public bool IsActive { get; private set; } = true;

    private EmployeeSchedule() { }

    private EmployeeSchedule(long employeeId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime) : base(0)
    {
        EmployeeId = employeeId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<EmployeeSchedule> Create(
        long employeeId,
        int dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            return Result.Failure<EmployeeSchedule>(
                new Error("EmployeeSchedule.InvalidDay", "Day of week must be 0-6."));

        if (startTime >= endTime)
            return Result.Failure<EmployeeSchedule>(
                new Error("EmployeeSchedule.InvalidTime", "Start time must be before end time."));

        return Result.Success(new EmployeeSchedule(employeeId, dayOfWeek, startTime, endTime));
    }
}
