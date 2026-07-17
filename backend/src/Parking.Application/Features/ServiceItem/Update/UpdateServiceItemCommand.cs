namespace Parking.Application.Features.ServiceItem.Update;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateServiceItemCommand(
    long ServiceItemId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal BaseCost) : ICommand<ServiceItemDto>;
