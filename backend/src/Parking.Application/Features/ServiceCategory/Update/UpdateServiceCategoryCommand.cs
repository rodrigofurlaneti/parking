namespace Parking.Application.Features.ServiceCategory.Update;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateServiceCategoryCommand(
    long ServiceCategoryId,
    string Name,
    string? Description) : ICommand<ServiceCategoryDto>;
