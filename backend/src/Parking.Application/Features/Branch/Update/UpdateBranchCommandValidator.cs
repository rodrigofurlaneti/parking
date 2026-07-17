namespace Parking.Application.Features.Branch.Update;

using FluentValidation;

internal sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.TotalSpaces).GreaterThan(0);
    }
}
