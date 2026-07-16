namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class WashSchedule : AggregateRoot
{
    public long BranchId { get; private set; }
    public long VehicleEntryId { get; private set; }
    public DateTime ScheduledTime { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public long EmployeeId { get; private set; }
    public int Status { get; private set; } // 0=Scheduled,1=InProgress,2=Completed,3=Cancelled
    public bool IsActive { get; private set; } = true;

    private WashSchedule() { }

    private WashSchedule(
        long branchId,
        long vehicleEntryId,
        DateTime scheduledTime,
        long employeeId) : base(0)
    {
        BranchId = branchId;
        VehicleEntryId = vehicleEntryId;
        ScheduledTime = scheduledTime;
        EmployeeId = employeeId;
        Status = 0;
    }

    public static Result<WashSchedule> Create(
        long branchId,
        long vehicleEntryId,
        DateTime scheduledTime,
        long employeeId)
    {
        if (branchId <= 0)
            return Result.Failure<WashSchedule>(new Error("WashSchedule.InvalidBranch", "Branch is required."));

        if (vehicleEntryId <= 0)
            return Result.Failure<WashSchedule>(new Error("WashSchedule.InvalidVehicleEntry", "Vehicle entry is required."));

        if (employeeId <= 0)
            return Result.Failure<WashSchedule>(new Error("WashSchedule.InvalidEmployee", "Employee is required."));

        return Result.Success(new WashSchedule(branchId, vehicleEntryId, scheduledTime, employeeId));
    }

    public Result AssignEmployee(long employeeId)
    {
        if (employeeId <= 0)
            return Result.Failure(new Error("WashSchedule.InvalidEmployee", "Employee is required."));

        if (Status is 2 or 3)
            return Result.Failure(new Error("WashSchedule.InvalidStatus", "Cannot assign employee to a finished schedule."));

        EmployeeId = employeeId;
        return Result.Success();
    }

    public Result Start()
    {
        if (Status != 0)
            return Result.Failure(new Error("WashSchedule.InvalidStatus", "Only scheduled washes can be started."));

        Status = 1;
        ActualStartTime = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != 1)
            return Result.Failure(new Error("WashSchedule.InvalidStatus", "Only washes in progress can be completed."));

        Status = 2;
        ActualEndTime = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is 2 or 3)
            return Result.Failure(new Error("WashSchedule.InvalidStatus", "Cannot cancel a finished wash."));

        Status = 3;
        return Result.Success();
    }
}
