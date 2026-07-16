namespace Parking.Application.Features.WashOperationalCost.GenerateReport;

using FluentValidation;

internal sealed class GenerateMonthlyReportCommandValidator : AbstractValidator<GenerateMonthlyReportCommand>
{
    public GenerateMonthlyReportCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.MonthYear).NotEmpty();
        RuleFor(x => x.LaborCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaterialCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EquipmentCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UtilitiesCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalRevenue).GreaterThanOrEqualTo(0);
    }
}
