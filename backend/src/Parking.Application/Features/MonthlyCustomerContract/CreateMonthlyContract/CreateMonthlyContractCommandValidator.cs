namespace Parking.Application.Features.MonthlyCustomerContract.CreateMonthlyContract;

using FluentValidation;

internal sealed class CreateMonthlyContractCommandValidator : AbstractValidator<CreateMonthlyContractCommand>
{
    public CreateMonthlyContractCommandValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.MonthlyFee).GreaterThan(0);
        RuleFor(x => x.MaxVehicles).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
    }
}
