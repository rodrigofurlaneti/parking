namespace Parking.Application.Features.ServiceItem.GetAll;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetServiceItemsQuery(long ServiceCategoryId) : IQuery<List<ServiceItemDto>>;
