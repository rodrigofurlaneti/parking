namespace Parking.Application.Features.ServiceCategory.Deactivate;

using Parking.Application.Abstractions.Messaging;

public sealed record DeactivateServiceCategoryCommand(long ServiceCategoryId) : ICommand;
