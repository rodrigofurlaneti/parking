namespace Parking.Application.Features.CashRegister.RecordCashMovement;

using FluentValidation;
using Parking.Domain.Entities;

internal sealed class RecordCashMovementCommandValidator : AbstractValidator<RecordCashMovementCommand>
{
    public RecordCashMovementCommandValidator()
    {
        RuleFor(x => x.CashRegisterId).GreaterThan(0);
        RuleFor(x => x.Type).InclusiveBetween(1, 3);

        // Entry/Exit must be strictly positive. Adjustment is the only type allowed to be negative
        // (e.g. correcting a cash count downward), but it must not be zero either. See
        // CashMovement.Create for the matching domain-level rule.
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.Type != CashMovement.Adjustment);
        RuleFor(x => x.Amount)
            .NotEqual(0)
            .When(x => x.Type == CashMovement.Adjustment);

        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
