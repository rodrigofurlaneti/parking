namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class WashProductConsumption : AggregateRoot
{
    public long WashScheduleId { get; private set; }
    public long ProductId { get; private set; }
    public decimal QuantityUsed { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public bool IsActive { get; private set; } = true;

    private WashProductConsumption() { }

    private WashProductConsumption(
        long washScheduleId,
        long productId,
        decimal quantityUsed,
        decimal unitCost) : base(0)
    {
        WashScheduleId = washScheduleId;
        ProductId = productId;
        QuantityUsed = quantityUsed;
        UnitCost = unitCost;
        TotalCost = quantityUsed * unitCost;
    }

    public static Result<WashProductConsumption> Create(
        long washScheduleId,
        long productId,
        decimal quantityUsed,
        decimal unitCost)
    {
        if (washScheduleId <= 0)
            return Result.Failure<WashProductConsumption>(new Error("WashProductConsumption.InvalidSchedule", "Wash schedule is required."));

        if (productId <= 0)
            return Result.Failure<WashProductConsumption>(new Error("WashProductConsumption.InvalidProduct", "Product is required."));

        if (quantityUsed <= 0)
            return Result.Failure<WashProductConsumption>(new Error("WashProductConsumption.InvalidQuantity", "Quantity must be greater than zero."));

        if (unitCost < 0)
            return Result.Failure<WashProductConsumption>(new Error("WashProductConsumption.InvalidUnitCost", "Unit cost cannot be negative."));

        return Result.Success(new WashProductConsumption(washScheduleId, productId, quantityUsed, unitCost));
    }
}
