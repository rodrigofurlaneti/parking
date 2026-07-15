namespace Parking.Application.Features.Branch.Create;

using FluentValidation;

internal sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.TotalSpaces).GreaterThan(0).LessThanOrEqualTo(10000);
    }
}
