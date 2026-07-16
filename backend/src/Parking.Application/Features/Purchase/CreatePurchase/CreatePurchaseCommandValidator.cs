namespace Parking.Application.Features.Purchase.CreatePurchase;

using FluentValidation;

internal sealed class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.SupplierId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(p => p.ProductId).GreaterThan(0);
            item.RuleFor(p => p.QuantityOrdered).GreaterThan(0);
            item.RuleFor(p => p.UnitCost).GreaterThanOrEqualTo(0);
        });
    }
}
