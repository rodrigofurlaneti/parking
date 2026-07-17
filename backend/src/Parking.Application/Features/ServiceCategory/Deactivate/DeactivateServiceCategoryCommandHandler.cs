namespace Parking.Application.Features.ServiceCategory.Deactivate;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class DeactivateServiceCategoryCommandHandler : ICommandHandler<DeactivateServiceCategoryCommand>
{
    private readonly IServiceCategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateServiceCategoryCommandHandler(IServiceCategoryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateServiceCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.ServiceCategoryId, cancellationToken);
        if (category is null)
            return Result.Failure(new Error("ServiceCategory.NotFound", "Service category not found."));

        category.Deactivate();
        await _repository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
