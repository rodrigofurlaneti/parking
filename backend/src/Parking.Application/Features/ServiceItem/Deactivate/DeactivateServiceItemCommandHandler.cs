namespace Parking.Application.Features.ServiceItem.Deactivate;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class DeactivateServiceItemCommandHandler : ICommandHandler<DeactivateServiceItemCommand>
{
    private readonly IServiceItemRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateServiceItemCommandHandler(IServiceItemRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateServiceItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.ServiceItemId, cancellationToken);
        if (item is null)
            return Result.Failure(new Error("ServiceItem.NotFound", "Service item not found."));

        item.Deactivate();
        await _repository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
