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

    public const int Entry = 1;
    public const int Exit = 2;
    public const int Adjustment = 3;

    public static Result<CashMovement> Create(
        long cashRegisterId,
        int type,
        decimal amount,
        string description)
    {
        if (type < 1 || type > 3)
            return Result.Failure<CashMovement>(
                new Error("CashMovement.InvalidType", "Type must be 1 (Entry), 2 (Exit), or 3 (Adjustment)."));

        // Entry/Exit movements must always be positive: the sign of their contribution to the cash
        // balance is implicit in the Type (Entry adds, Exit subtracts). Adjustment movements are the
        // one case where the operator needs to correct the expected balance in either direction (e.g.
        // fixing a cash count that was recorded too high), so Amount is allowed to be negative for
        // Type == Adjustment, but never zero. This keeps GetCashReportQueryHandler's formula
        // (OpeningBalance + Entries - Exits + Adjustments) correct without conditional logic, since a
        // negative adjustment already subtracts itself when summed.
        if (type == Adjustment)
        {
            if (amount == 0)
                return Result.Failure<CashMovement>(
                    new Error("CashMovement.InvalidAmount", "Adjustment amount must not be zero."));
        }
        else if (amount <= 0)
        {
            return Result.Failure<CashMovement>(
                new Error("CashMovement.InvalidAmount", "Amount must be greater than 0."));
        }

        return Result.Success(new CashMovement(cashRegisterId, type, amount, description));
    }
}
