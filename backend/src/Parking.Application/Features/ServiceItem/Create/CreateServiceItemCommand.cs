namespace Parking.Application.Features.ServiceItem.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateServiceItemCommand(
    long ServiceCategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal BaseCost) : ICommand<ServiceItemDto>;
