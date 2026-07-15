namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class VehicleExit : Entity
{
    public long VehicleEntryId { get; private set; }
    public DateTime ExitTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal TotalAmount { get; private set; }
    public int ParkingMode { get; private set; } // 1=Rotativo, 2=Agreement, 3=Monthly
    public bool IsActive { get; private set; } = true;

    private VehicleExit() { }

    private VehicleExit(
        long vehicleEntryId,
        int durationMinutes,
        decimal totalAmount,
        int parkingMode) : base(0)
    {
        VehicleEntryId = vehicleEntryId;
        ExitTime = DateTime.UtcNow;
        DurationMinutes = durationMinutes;
        TotalAmount = totalAmount;
        ParkingMode = parkingMode;
    }

    public static Result<VehicleExit> Create(
        long vehicleEntryId,
        int durationMinutes,
        decimal totalAmount,
        int parkingMode)
    {
        if (durationMinutes <= 0)
            return Result.Failure<VehicleExit>(
                new Error("VehicleExit.InvalidDuration", "Duration must be greater than 0."));

        if (totalAmount < 0)
            return Result.Failure<VehicleExit>(
                new Error("VehicleExit.InvalidAmount", "Amount cannot be negative."));

        return Result.Success(new VehicleExit(vehicleEntryId, durationMinutes, totalAmount, parkingMode));
    }
}
