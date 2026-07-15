namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class CashMovement : Entity
{
    public long CashRegisterId { get; private set; }
    public int Type { get; private set; } // 1=Entry, 2=Exit, 3=Adjustment
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = null!;
    public int? ReferencedDocumentType { get; private set; }
    public long? ReferencedDocumentId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private CashMovement() { }

    private CashMovement(long cashRegisterId, int type, decimal amount, string description) : base(0)
    {
        CashRegisterId = cashRegisterId;
        Type = type;
        Amount = amount;
        Description = description;
    }

    public static Result<CashMovement> Create(
        long cashRegisterId,
        int type,
        decimal amount,
        string description)
    {
        if (amount <= 0)
            return Result.Failure<CashMovement>(
                new Error("CashMovement.InvalidAmount", "Amount must be greater than 0."));

        if (type < 1 || type > 3)
            return Result.Failure<CashMovement>(
                new Error("CashMovement.InvalidType", "Type must be 1 (Entry), 2 (Exit), or 3 (Adjustment)."));

        return Result.Success(new CashMovement(cashRegisterId, type, amount, description));
    }
}
