namespace Parking.Application.Features.WashSchedule.Create;

using FluentValidation;

internal sealed class CreateWashScheduleCommandValidator : AbstractValidator<CreateWashScheduleCommand>
{
    public CreateWashScheduleCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.VehicleEntryId).GreaterThan(0);
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.ScheduledTime).NotEmpty();
    }
}
