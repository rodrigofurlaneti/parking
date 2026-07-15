namespace Parking.Application.Features.Employee.GeneratePayroll;

using FluentValidation;

internal sealed class GeneratePayrollCommandValidator : AbstractValidator<GeneratePayrollCommand>
{
    public GeneratePayrollCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.MonthYear).NotEmpty();
        RuleFor(x => x.BaseSalary).GreaterThan(0);
    }
}
