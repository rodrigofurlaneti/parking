namespace Parking.Application.Features.CashRegister.CloseCashRegister;

using FluentValidation;

internal sealed class CloseCashRegisterCommandValidator : AbstractValidator<CloseCashRegisterCommand>
{
    public CloseCashRegisterCommandValidator()
    {
        RuleFor(x => x.CashRegisterId).GreaterThan(0);
        RuleFor(x => x.ClosingBalance).GreaterThanOrEqualTo(0);
    }
}
