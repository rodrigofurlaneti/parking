namespace Parking.Application.Features.AgreementMerchant.CreateAgreementMerchant;

using FluentValidation;

internal sealed class CreateAgreementMerchantCommandValidator : AbstractValidator<CreateAgreementMerchantCommand>
{
    public CreateAgreementMerchantCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DiscountPercentage).InclusiveBetween(0, 100);
    }
}
