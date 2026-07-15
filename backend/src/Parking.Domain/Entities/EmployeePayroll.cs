namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class EmployeePayroll : Entity
{
    public long EmployeeId { get; private set; }
    public DateTime MonthYear { get; private set; }
    public decimal BaseSalary { get; private set; }
    public decimal Bonuses { get; private set; } = 0m;
    public decimal Deductions { get; private set; } = 0m;
    public int Status { get; private set; } = 0; // 0=Draft, 1=Approved, 2=Paid
    public DateTime? PaidDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private EmployeePayroll() { }

    private EmployeePayroll(long employeeId, DateTime monthYear, decimal baseSalary) : base(0)
    {
        EmployeeId = employeeId;
        MonthYear = monthYear;
        BaseSalary = baseSalary;
    }

    public static Result<EmployeePayroll> Create(long employeeId, DateTime monthYear, decimal baseSalary)
    {
        if (baseSalary <= 0)
            return Result.Failure<EmployeePayroll>(
                new Error("EmployeePayroll.InvalidSalary", "Salary must be greater than 0."));

        return Result.Success(new EmployeePayroll(employeeId, monthYear, baseSalary));
    }

    public decimal GetTotalAmount() => BaseSalary + Bonuses - Deductions;

    public void Approve() => Status = 1;
    public void MarkAsPaid() => Status = 2;
}
