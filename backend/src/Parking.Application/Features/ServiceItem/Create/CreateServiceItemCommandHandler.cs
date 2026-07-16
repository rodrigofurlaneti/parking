namespace Parking.Application.Features.ServiceItem.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainServiceItem = Parking.Domain.Entities.ServiceItem;

internal sealed class CreateServiceItemCommandHandler : ICommandHandler<CreateServiceItemCommand, ServiceItemDto>
{
    private readonly IServiceItemRepository _serviceItemRepository;
    private readonly IServiceCategoryRepository _serviceCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceItemCommandHandler(
        IServiceItemRepository serviceItemRepository,
        IServiceCategoryRepository serviceCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceItemRepository = serviceItemRepository;
        _serviceCategoryRepository = serviceCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ServiceItemDto>> Handle(CreateServiceItemCommand request, CancellationToken cancellationToken)
    {
        var category = await _serviceCategoryRepository.GetByIdAsync(request.ServiceCategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<ServiceItemDto>(
                new Error("ServiceCategory.NotFound", "Service category not found."));

        var itemResult = DomainServiceItem.Create(
            request.ServiceCategoryId,
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.BaseCost);

        if (itemResult.IsFailure)
            return Result.Failure<ServiceItemDto>(itemResult.Error);

        await _serviceItemRepository.AddAsync(itemResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ServiceItemDto(
            itemResult.Value.Id,
            itemResult.Value.ServiceCategoryId,
            itemResult.Value.Name,
            itemResult.Value.Description,
            itemResult.Value.DurationMinutes,
            itemResult.Value.BaseCost,
            itemResult.Value.IsActive));
    }
}
