namespace Parking.Application.Features.ServiceCategory.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainServiceCategory = Parking.Domain.Entities.ServiceCategory;

internal sealed class CreateServiceCategoryCommandHandler : ICommandHandler<CreateServiceCategoryCommand, ServiceCategoryDto>
{
    private readonly IServiceCategoryRepository _serviceCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceCategoryCommandHandler(
        IServiceCategoryRepository serviceCategoryRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceCategoryRepository = serviceCategoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ServiceCategoryDto>> Handle(CreateServiceCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryResult = DomainServiceCategory.Create(request.BranchId, request.Name, request.Description);

        if (categoryResult.IsFailure)
            return Result.Failure<ServiceCategoryDto>(categoryResult.Error);

        await _serviceCategoryRepository.AddAsync(categoryResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ServiceCategoryDto(
            categoryResult.Value.Id,
            categoryResult.Value.BranchId,
            categoryResult.Value.Name,
            categoryResult.Value.Description,
            categoryResult.Value.IsActive));
    }
}
