namespace Parking.Application.Features.VehicleEntry.RegisterVehicleExit;

using FluentValidation;

internal sealed class RegisterVehicleExitCommandValidator : AbstractValidator<RegisterVehicleExitCommand>
{
    public RegisterVehicleExitCommandValidator()
    {
        RuleFor(x => x.VehicleEntryId).GreaterThan(0);
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ParkingMode).InclusiveBetween(1, 3);
    }
}
