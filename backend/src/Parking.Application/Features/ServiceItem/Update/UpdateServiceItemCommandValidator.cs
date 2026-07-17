namespace Parking.Application.Features.ServiceItem.Update;

using FluentValidation;

internal sealed class UpdateServiceItemCommandValidator : AbstractValidator<UpdateServiceItemCommand>
{
    public UpdateServiceItemCommandValidator()
    {
        RuleFor(x => x.ServiceItemId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.BaseCost).GreaterThanOrEqualTo(0);
    }
}
