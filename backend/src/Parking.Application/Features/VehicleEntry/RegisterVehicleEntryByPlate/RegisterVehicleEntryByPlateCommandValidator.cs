namespace Parking.Application.Features.VehicleEntry.RegisterVehicleEntryByPlate;

using FluentValidation;

internal sealed class RegisterVehicleEntryByPlateCommandValidator : AbstractValidator<RegisterVehicleEntryByPlateCommand>
{
    public RegisterVehicleEntryByPlateCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.ParkingSpaceId).GreaterThan(0);
        RuleFor(x => x.LicensePlate).NotEmpty().MaximumLength(10);
        RuleFor(x => x.VehicleModel).MaximumLength(255);
        RuleFor(x => x.VehicleColor).MaximumLength(50);
    }
}
