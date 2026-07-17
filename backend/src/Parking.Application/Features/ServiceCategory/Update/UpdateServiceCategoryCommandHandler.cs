namespace Parking.Application.Features.ServiceCategory.Update;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateServiceCategoryCommandHandler : ICommandHandler<UpdateServiceCategoryCommand, ServiceCategoryDto>
{
    private readonly IServiceCategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceCategoryCommandHandler(IServiceCategoryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ServiceCategoryDto>> Handle(UpdateServiceCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.ServiceCategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<ServiceCategoryDto>(new Error("ServiceCategory.NotFound", "Service category not found."));

        var updateResult = category.Update(request.Name, request.Description);
        if (updateResult.IsFailure)
            return Result.Failure<ServiceCategoryDto>(updateResult.Error);

        await _repository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ServiceCategoryDto(
            category.Id, category.BranchId, category.Name, category.Description, category.IsActive));
    }
}
