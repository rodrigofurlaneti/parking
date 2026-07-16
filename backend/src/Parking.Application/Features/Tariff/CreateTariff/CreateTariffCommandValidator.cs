namespace Parking.Application.Features.Tariff.CreateTariff;

using FluentValidation;

internal sealed class CreateTariffCommandValidator : AbstractValidator<CreateTariffCommand>
{
    public CreateTariffCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.FirstHourRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AdditionalHourRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DailyMaxRate).GreaterThan(0).When(x => x.DailyMaxRate is not null);
    }
}
