namespace Parking.Application.Features.Employee.GeneratePayroll;

using Parking.Application.Abstractions.Messaging;

public sealed record GeneratePayrollCommand(
    long EmployeeId,
    DateTime MonthYear,
    decimal BaseSalary) : ICommand<long>;
