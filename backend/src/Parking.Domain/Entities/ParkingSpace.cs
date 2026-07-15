namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class ParkingSpace : AggregateRoot
{
    public long BranchId { get; private set; }
    public string SpaceNumber { get; private set; } = null!;
    public int Type { get; private set; } // 1=Covered, 2=Uncovered, 3=Reserved, 4=Handicap
    public int Status { get; private set; } = 0; // 0=Available, 1=Occupied, 2=Maintenance
    public bool IsActive { get; private set; } = true;

    private ParkingSpace() { }

    private ParkingSpace(long branchId, string spaceNumber, int type) : base(0)
    {
        BranchId = branchId;
        SpaceNumber = spaceNumber;
        Type = type;
    }

    public static Result<ParkingSpace> Create(long branchId, string spaceNumber, int type)
    {
        if (string.IsNullOrWhiteSpace(spaceNumber))
            return Result.Failure<ParkingSpace>(
                new Error("ParkingSpace.InvalidNumber", "Space number required."));

        if (type < 1 || type > 4)
            return Result.Failure<ParkingSpace>(
                new Error("ParkingSpace.InvalidType", "Type must be 1-4."));

        return Result.Success(new ParkingSpace(branchId, spaceNumber, type));
    }

    public void MarkAsOccupied() => Status = 1;
    public void MarkAsAvailable() => Status = 0;
    public void MarkAsMaintenance() => Status = 2;
}
