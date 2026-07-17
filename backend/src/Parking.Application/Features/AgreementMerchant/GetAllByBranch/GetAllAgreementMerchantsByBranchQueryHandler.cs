namespace Parking.Application.Features.AgreementMerchant.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllAgreementMerchantsByBranchQueryHandler
    : IQueryHandler<GetAllAgreementMerchantsByBranchQuery, List<AgreementMerchantDto>>
{
    private readonly IAgreementMerchantRepository _repository;

    public GetAllAgreementMerchantsByBranchQueryHandler(IAgreementMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<AgreementMerchantDto>>> Handle(GetAllAgreementMerchantsByBranchQuery request, CancellationToken cancellationToken)
    {
        var merchants = await _repository.GetAllByBranchAsync(request.BranchId, cancellationToken);
        var dtos = merchants.Select(x => new AgreementMerchantDto(
            x.Id, x.BranchId, x.CompanyName, x.DiscountPercentage, x.IsActive)).ToList();
        return Result.Success(dtos);
    }
}
