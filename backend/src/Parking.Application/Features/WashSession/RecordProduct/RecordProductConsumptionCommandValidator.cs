namespace Parking.Application.Features.WashSession.RecordProduct;

using FluentValidation;

internal sealed class RecordProductConsumptionCommandValidator : AbstractValidator<RecordProductConsumptionCommand>
{
    public RecordProductConsumptionCommandValidator()
    {
        RuleFor(x => x.WashScheduleId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.QuantityUsed).GreaterThan(0);
    }
}
