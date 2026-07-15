namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class CashRegister : AggregateRoot
{
    public long BranchId { get; private set; }
    public long EmployeeId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public decimal ClosingBalance { get; private set; }
    public int Status { get; private set; } = 0; // 0=Open, 1=Closed
    public bool IsActive { get; private set; } = true;

    private CashRegister() { }

    private CashRegister(long branchId, long employeeId, decimal openingBalance) : base(0)
    {
        BranchId = branchId;
        EmployeeId = employeeId;
        OpeningBalance = openingBalance;
        OpenedAt = DateTime.UtcNow;
    }

    public static Result<CashRegister> Create(long branchId, long employeeId, decimal openingBalance)
    {
        if (openingBalance < 0)
            return Result.Failure<CashRegister>(
                new Error("CashRegister.InvalidBalance", "Opening balance cannot be negative."));

        return Result.Success(new CashRegister(branchId, employeeId, openingBalance));
    }

    public void Close(decimal closingBalance)
    {
        ClosingBalance = closingBalance;
        ClosedAt = DateTime.UtcNow;
        Status = 1;
    }
}
