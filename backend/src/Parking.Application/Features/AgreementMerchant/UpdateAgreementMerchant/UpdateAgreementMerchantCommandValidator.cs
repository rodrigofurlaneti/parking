namespace Parking.Application.Features.AgreementMerchant.UpdateAgreementMerchant;

using FluentValidation;

internal sealed class UpdateAgreementMerchantCommandValidator : AbstractValidator<UpdateAgreementMerchantCommand>
{
    public UpdateAgreementMerchantCommandValidator()
    {
        RuleFor(x => x.AgreementMerchantId).GreaterThan(0);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.DiscountPercentage).InclusiveBetween(0, 100);
    }
}
