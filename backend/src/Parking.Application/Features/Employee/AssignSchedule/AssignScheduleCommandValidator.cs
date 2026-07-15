namespace Parking.Application.Features.Employee.AssignSchedule;

using FluentValidation;

internal sealed class AssignScheduleCommandValidator : AbstractValidator<AssignScheduleCommand>
{
    public AssignScheduleCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.DayOfWeek).InclusiveBetween(0, 6);
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty().GreaterThan(x => x.StartTime);
    }
}
