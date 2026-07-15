namespace Parking.Application.Features.Employee.GetPayroll;

using Parking.Application.Abstractions.Messaging;

public sealed record GetPayrollQuery(long EmployeeId) : IQuery<List<EmployeePayrollDto>>;

public sealed record EmployeePayrollDto(
    long Id,
    long EmployeeId,
    DateTime MonthYear,
    decimal BaseSalary,
    decimal Bonuses,
    decimal Deductions,
    decimal TotalAmount,
    int Status,
    DateTime? PaidDate,
    bool IsActive);
