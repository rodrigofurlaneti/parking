namespace Parking.Application.Features.ServiceCategory.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllServiceCategoriesByBranchQueryHandler : IQueryHandler<GetAllServiceCategoriesByBranchQuery, List<ServiceCategoryDto>>
{
    private readonly IServiceCategoryRepository _serviceCategoryRepository;

    public GetAllServiceCategoriesByBranchQueryHandler(IServiceCategoryRepository serviceCategoryRepository)
    {
        _serviceCategoryRepository = serviceCategoryRepository;
    }

    public async Task<Result<List<ServiceCategoryDto>>> Handle(GetAllServiceCategoriesByBranchQuery request, CancellationToken cancellationToken)
    {
        var categories = await _serviceCategoryRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = categories.Select(x => new ServiceCategoryDto(
            x.Id,
            x.BranchId,
            x.Name,
            x.Description,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
