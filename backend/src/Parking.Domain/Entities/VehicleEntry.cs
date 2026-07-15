namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class VehicleEntry : AggregateRoot
{
    public long BranchId { get; private set; }
    public long ParkingSpaceId { get; private set; }
    public long CustomerId { get; private set; }
    public string LicensePlate { get; private set; } = null!;
    public string VehicleModel { get; private set; } = null!;
    public string VehicleColor { get; private set; } = null!;
    public DateTime EntryTime { get; private set; }
    public DateTime? ExitTime { get; private set; }
    public int Status { get; private set; } = 0; // 0=Parked, 1=Exited
    public bool IsActive { get; private set; } = true;

    private VehicleEntry() { }

    private VehicleEntry(
        long branchId,
        long parkingSpaceId,
        long customerId,
        string licensePlate,
        string vehicleModel,
        string vehicleColor) : base(0)
    {
        BranchId = branchId;
        ParkingSpaceId = parkingSpaceId;
        CustomerId = customerId;
        LicensePlate = licensePlate;
        VehicleModel = vehicleModel;
        VehicleColor = vehicleColor;
        EntryTime = DateTime.UtcNow;
    }

    public static Result<VehicleEntry> Create(
        long branchId,
        long parkingSpaceId,
        long customerId,
        string licensePlate,
        string vehicleModel,
        string vehicleColor)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return Result.Failure<VehicleEntry>(
                new Error("VehicleEntry.InvalidPlate", "License plate required."));

        return Result.Success(new VehicleEntry(branchId, parkingSpaceId, customerId, licensePlate, vehicleModel, vehicleColor));
    }

    public void MarkAsExited()
    {
        ExitTime = DateTime.UtcNow;
        Status = 1;
    }

    public int GetDurationMinutes()
    {
        var exitTime = ExitTime ?? DateTime.UtcNow;
        return (int)(exitTime - EntryTime).TotalMinutes;
    }
}
