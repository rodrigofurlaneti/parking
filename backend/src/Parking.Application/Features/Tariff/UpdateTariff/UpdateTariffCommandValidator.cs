namespace Parking.Application.Features.Tariff.UpdateTariff;

using FluentValidation;

internal sealed class UpdateTariffCommandValidator : AbstractValidator<UpdateTariffCommand>
{
    public UpdateTariffCommandValidator()
    {
        RuleFor(x => x.TariffId).GreaterThan(0);
        RuleFor(x => x.FirstHourRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AdditionalHourRate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DailyMaxRate).GreaterThan(0).When(x => x.DailyMaxRate is not null);
    }
}
