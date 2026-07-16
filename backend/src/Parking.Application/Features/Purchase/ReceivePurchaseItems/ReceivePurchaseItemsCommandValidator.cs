namespace Parking.Application.Features.Purchase.ReceivePurchaseItems;

using FluentValidation;

internal sealed class ReceivePurchaseItemsCommandValidator : AbstractValidator<ReceivePurchaseItemsCommand>
{
    public ReceivePurchaseItemsCommandValidator()
    {
        RuleFor(x => x.PurchaseId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(p => p.PurchaseItemId).GreaterThan(0);
            item.RuleFor(p => p.QuantityReceived).GreaterThan(0);
        });
    }
}
