namespace Parking.Application.Features.Sale.RegisterSale;

using FluentValidation;

internal sealed class RegisterSaleCommandValidator : AbstractValidator<RegisterSaleCommand>
{
    public RegisterSaleCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.VehicleExitId).GreaterThan(0);
        RuleFor(x => x.CashRegisterId).GreaterThan(0);
        RuleFor(x => x.Payments).NotEmpty();
        RuleForEach(x => x.Payments).ChildRules(payment =>
        {
            payment.RuleFor(p => p.PaymentMethod).InclusiveBetween(1, 5);
            payment.RuleFor(p => p.Amount).GreaterThan(0);
        });
    }
}
