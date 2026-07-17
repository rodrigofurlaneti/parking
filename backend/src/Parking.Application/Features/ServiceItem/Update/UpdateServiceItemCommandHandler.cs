namespace Parking.Application.Features.ServiceItem.Update;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateServiceItemCommandHandler : ICommandHandler<UpdateServiceItemCommand, ServiceItemDto>
{
    private readonly IServiceItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceItemCommandHandler(IServiceItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ServiceItemDto>> Handle(UpdateServiceItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.ServiceItemId, cancellationToken);
        if (item is null)
            return Result.Failure<ServiceItemDto>(new Error("ServiceItem.NotFound", "Service item not found."));

        var updateResult = item.Update(request.Name, request.Description, request.DurationMinutes, request.BaseCost);
        if (updateResult.IsFailure)
            return Result.Failure<ServiceItemDto>(updateResult.Error);

        await _repository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ServiceItemDto(
            item.Id, item.ServiceCategoryId, item.Name, item.Description, item.DurationMinutes, item.BaseCost, item.IsActive));
    }
}
