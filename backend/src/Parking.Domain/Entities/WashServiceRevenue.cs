namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class WashServiceRevenue : AggregateRoot
{
    public long WashScheduleId { get; private set; }
    public long ServiceItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal Commission { get; private set; }
    public DateTime Date { get; private set; }
    public bool IsActive { get; private set; } = true;

    private WashServiceRevenue() { }

    private WashServiceRevenue(
        long washScheduleId,
        long serviceItemId,
        int quantity,
        decimal unitPrice,
        decimal commission) : base(0)
    {
        WashScheduleId = washScheduleId;
        ServiceItemId = serviceItemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = quantity * unitPrice;
        Commission = commission;
        Date = DateTime.UtcNow;
    }

    public static Result<WashServiceRevenue> Create(
        long washScheduleId,
        long serviceItemId,
        int quantity,
        decimal unitPrice,
        decimal commission)
    {
        if (washScheduleId <= 0)
            return Result.Failure<WashServiceRevenue>(new Error("WashServiceRevenue.InvalidSchedule", "Wash schedule is required."));

        if (serviceItemId <= 0)
            return Result.Failure<WashServiceRevenue>(new Error("WashServiceRevenue.InvalidServiceItem", "Service item is required."));

        if (quantity <= 0)
            return Result.Failure<WashServiceRevenue>(new Error("WashServiceRevenue.InvalidQuantity", "Quantity must be greater than zero."));

        if (unitPrice < 0)
            return Result.Failure<WashServiceRevenue>(new Error("WashServiceRevenue.InvalidUnitPrice", "Unit price cannot be negative."));

        if (commission < 0)
            return Result.Failure<WashServiceRevenue>(new Error("WashServiceRevenue.InvalidCommission", "Commission cannot be negative."));

        return Result.Success(new WashServiceRevenue(washScheduleId, serviceItemId, quantity, unitPrice, commission));
    }
}
