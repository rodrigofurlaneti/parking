namespace Parking.Application.Features.ParkingSpace.CreateParkingSpace;

using FluentValidation;

internal sealed class CreateParkingSpaceCommandValidator : AbstractValidator<CreateParkingSpaceCommand>
{
    public CreateParkingSpaceCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.SpaceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Type).InclusiveBetween(1, 4);
    }
}
