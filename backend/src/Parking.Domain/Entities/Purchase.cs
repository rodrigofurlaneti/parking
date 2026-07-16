namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Purchase : AggregateRoot
{
    public const int Pending = 0;
    public const int PartiallyReceived = 1;
    public const int Received = 2;

    public long BranchId { get; private set; }
    public long SupplierId { get; private set; }
    public long PurchaseNumber { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public int Status { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Purchase() { }

    private Purchase(
        long branchId,
        long supplierId,
        long purchaseNumber) : base(0)
    {
        BranchId = branchId;
        SupplierId = supplierId;
        PurchaseNumber = purchaseNumber;
        PurchaseDate = DateTime.UtcNow;
        Status = Pending;
    }

    public static Result<Purchase> Create(
        long branchId,
        long supplierId,
        long purchaseNumber)
    {
        if (branchId <= 0)
            return Result.Failure<Purchase>(new Error("Purchase.InvalidBranch", "Branch is required."));

        if (supplierId <= 0)
            return Result.Failure<Purchase>(new Error("Purchase.InvalidSupplier", "Supplier is required."));

        if (purchaseNumber <= 0)
            return Result.Failure<Purchase>(new Error("Purchase.InvalidNumber", "Purchase number must be greater than zero."));

        return Result.Success(new Purchase(branchId, supplierId, purchaseNumber));
    }

    public void MarkAsReceived()
    {
        Status = Received;
    }

    public void MarkAsPartiallyReceived()
    {
        Status = PartiallyReceived;
    }
}
