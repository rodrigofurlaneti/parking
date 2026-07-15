namespace Parking.Application.Features.Company.Create;

using FluentValidation;

internal sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Cnpj).NotEmpty().Length(14);
        RuleFor(x => x.LegalName).NotEmpty().MaximumLength(255);
    }
}
