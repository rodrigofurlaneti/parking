namespace Parking.Application.Features.ServiceItem.Deactivate;

using Parking.Application.Abstractions.Messaging;

public sealed record DeactivateServiceItemCommand(long ServiceItemId) : ICommand;
