namespace Parking.Application.Features.ServiceItem.Create;

using FluentValidation;

internal sealed class CreateServiceItemCommandValidator : AbstractValidator<CreateServiceItemCommand>
{
    public CreateServiceItemCommandValidator()
    {
        RuleFor(x => x.ServiceCategoryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.BaseCost).GreaterThanOrEqualTo(0);
    }
}
