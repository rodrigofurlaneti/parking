namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Sale : AggregateRoot
{
    public long BranchId { get; private set; }
    public long VehicleExitId { get; private set; }
    public long CashRegisterId { get; private set; }
    public long SaleNumber { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime SaleDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Sale() { }

    private Sale(
        long branchId,
        long vehicleExitId,
        long cashRegisterId,
        long saleNumber,
        decimal totalAmount) : base(0)
    {
        BranchId = branchId;
        VehicleExitId = vehicleExitId;
        CashRegisterId = cashRegisterId;
        SaleNumber = saleNumber;
        TotalAmount = totalAmount;
        SaleDate = DateTime.UtcNow;
    }

    public static Result<Sale> Create(
        long branchId,
        long vehicleExitId,
        long cashRegisterId,
        long saleNumber,
        decimal totalAmount)
    {
        if (totalAmount <= 0)
            return Result.Failure<Sale>(
                new Error("Sale.InvalidAmount", "Total amount must be greater than 0."));

        return Result.Success(new Sale(branchId, vehicleExitId, cashRegisterId, saleNumber, totalAmount));
    }

    public Result Refund()
    {
        if (!IsActive)
            return Result.Failure(new Error("Sale.AlreadyRefunded", "Sale is already refunded."));

        IsActive = false;
        return Result.Success();
    }
}
