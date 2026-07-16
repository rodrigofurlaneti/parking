namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class WashSession : AggregateRoot
{
    public long WashScheduleId { get; private set; }
    public decimal TotalCost { get; private set; }
    public int TotalDurationMinutes { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public int Status { get; private set; } // 0=InProgress,1=Completed,2=Cancelled
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;

    private WashSession() { }

    private WashSession(long washScheduleId, string? notes) : base(0)
    {
        WashScheduleId = washScheduleId;
        StartTime = DateTime.UtcNow;
        Status = 0;
        Notes = notes;
    }

    public static Result<WashSession> Create(long washScheduleId, string? notes = null)
    {
        if (washScheduleId <= 0)
            return Result.Failure<WashSession>(new Error("WashSession.InvalidSchedule", "Wash schedule is required."));

        return Result.Success(new WashSession(washScheduleId, notes));
    }

    public void AddCost(decimal cost, int durationMinutes)
    {
        TotalCost += cost;
        TotalDurationMinutes += durationMinutes;
    }

    public Result Complete()
    {
        if (Status != 0)
            return Result.Failure(new Error("WashSession.InvalidStatus", "Only sessions in progress can be completed."));

        Status = 1;
        EndTime = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == 1)
            return Result.Failure(new Error("WashSession.InvalidStatus", "Cannot cancel a completed session."));

        Status = 2;
        return Result.Success();
    }
}
