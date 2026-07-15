namespace Parking.Application.Features.CashRegister.OpenCashRegister;

using FluentValidation;

internal sealed class OpenCashRegisterCommandValidator : AbstractValidator<OpenCashRegisterCommand>
{
    public OpenCashRegisterCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.OpeningBalance).GreaterThanOrEqualTo(0);
    }
}
