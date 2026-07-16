namespace Parking.Application.Features.ServiceItem.GetAll;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetServiceItemsQueryHandler : IQueryHandler<GetServiceItemsQuery, List<ServiceItemDto>>
{
    private readonly IServiceItemRepository _serviceItemRepository;

    public GetServiceItemsQueryHandler(IServiceItemRepository serviceItemRepository)
    {
        _serviceItemRepository = serviceItemRepository;
    }

    public async Task<Result<List<ServiceItemDto>>> Handle(GetServiceItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _serviceItemRepository.GetAllByCategoryAsync(request.ServiceCategoryId, cancellationToken);

        var dtos = items.Select(x => new ServiceItemDto(
            x.Id,
            x.ServiceCategoryId,
            x.Name,
            x.Description,
            x.DurationMinutes,
            x.BaseCost,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
