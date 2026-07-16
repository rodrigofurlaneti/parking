namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class PurchaseItem : Entity
{
    public long PurchaseId { get; private set; }
    public long ProductId { get; private set; }
    public decimal QuantityOrdered { get; private set; }
    public decimal QuantityReceived { get; private set; }
    public decimal UnitCost { get; private set; }

    public bool IsFullyReceived => QuantityReceived >= QuantityOrdered;

    private PurchaseItem() { }

    private PurchaseItem(
        long purchaseId,
        long productId,
        decimal quantityOrdered,
        decimal unitCost) : base(0)
    {
        PurchaseId = purchaseId;
        ProductId = productId;
        QuantityOrdered = quantityOrdered;
        QuantityReceived = 0;
        UnitCost = unitCost;
    }

    public static Result<PurchaseItem> Create(
        long purchaseId,
        long productId,
        decimal quantityOrdered,
        decimal unitCost)
    {
        if (purchaseId <= 0)
            return Result.Failure<PurchaseItem>(new Error("PurchaseItem.InvalidPurchase", "Purchase is required."));

        if (productId <= 0)
            return Result.Failure<PurchaseItem>(new Error("PurchaseItem.InvalidProduct", "Product is required."));

        if (quantityOrdered <= 0)
            return Result.Failure<PurchaseItem>(new Error("PurchaseItem.InvalidQuantity", "Quantity ordered must be greater than zero."));

        if (unitCost < 0)
            return Result.Failure<PurchaseItem>(new Error("PurchaseItem.InvalidUnitCost", "Unit cost cannot be negative."));

        return Result.Success(new PurchaseItem(purchaseId, productId, quantityOrdered, unitCost));
    }

    public Result ReceiveQuantity(decimal quantity)
    {
        if (quantity <= 0)
            return Result.Failure(new Error("PurchaseItem.InvalidQuantity", "Quantity must be greater than zero."));

        if (QuantityReceived + quantity > QuantityOrdered)
            return Result.Failure(new Error("PurchaseItem.ExceedsOrdered", "Received quantity exceeds ordered quantity."));

        QuantityReceived += quantity;
        return Result.Success();
    }
}
