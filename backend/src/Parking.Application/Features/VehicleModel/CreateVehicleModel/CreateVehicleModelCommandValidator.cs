namespace Parking.Application.Features.VehicleModel.CreateVehicleModel;

using FluentValidation;

internal sealed class CreateVehicleModelCommandValidator : AbstractValidator<CreateVehicleModelCommand>
{
    public CreateVehicleModelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
