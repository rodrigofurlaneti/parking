namespace Parking.Application.Features.ServiceCategory.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateServiceCategoryCommand(
    long BranchId,
    string Name,
    string? Description) : ICommand<ServiceCategoryDto>;
