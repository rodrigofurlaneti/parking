namespace Parking.Application.Features.Branch.GetByCompany;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetBranchesByCompanyQueryHandler : IQueryHandler<GetBranchesByCompanyQuery, List<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;

    public GetBranchesByCompanyQueryHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task<Result<List<BranchDto>>> Handle(GetBranchesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllByCompanyAsync(request.CompanyId, cancellationToken);

        var dtos = branches.Select(b => new BranchDto(
            b.Id,
            b.CompanyId,
            b.Name,
            b.TotalSpaces,
            b.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
