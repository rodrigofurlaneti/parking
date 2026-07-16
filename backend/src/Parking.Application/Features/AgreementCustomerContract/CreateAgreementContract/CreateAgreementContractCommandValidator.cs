namespace Parking.Application.Features.AgreementCustomerContract.CreateAgreementContract;

using FluentValidation;

internal sealed class CreateAgreementContractCommandValidator : AbstractValidator<CreateAgreementContractCommand>
{
    public CreateAgreementContractCommandValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.AgreementMerchantId).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
    }
}
