namespace Parking.Application.Features.Vehicle.CreateVehicle;

using FluentValidation;

internal sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.LicensePlate).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.Color).MaximumLength(50);
    }
}
