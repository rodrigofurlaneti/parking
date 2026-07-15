namespace Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;

using FluentValidation;

internal sealed class RegisterVehicleEntryCommandValidator : AbstractValidator<RegisterVehicleEntryCommand>
{
    public RegisterVehicleEntryCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.ParkingSpaceId).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.LicensePlate).NotEmpty().MaximumLength(20);
        RuleFor(x => x.VehicleModel).NotEmpty().MaximumLength(255);
        RuleFor(x => x.VehicleColor).NotEmpty().MaximumLength(50);
    }
}
