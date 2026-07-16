namespace Parking.Application.Features.WashSchedule.AssignEmployee;

using FluentValidation;

internal sealed class AssignWashEmployeeCommandValidator : AbstractValidator<AssignWashEmployeeCommand>
{
    public AssignWashEmployeeCommandValidator()
    {
        RuleFor(x => x.WashScheduleId).GreaterThan(0);
        RuleFor(x => x.EmployeeId).GreaterThan(0);
    }
}
