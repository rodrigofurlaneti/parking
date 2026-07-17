namespace Parking.Application.Features.Tariff.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllTariffsByBranchQueryHandler : IQueryHandler<GetAllTariffsByBranchQuery, List<TariffDto>>
{
    private readonly ITariffRepository _repository;

    public GetAllTariffsByBranchQueryHandler(ITariffRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<TariffDto>>> Handle(GetAllTariffsByBranchQuery request, CancellationToken cancellationToken)
    {
        var tariffs = await _repository.GetAllByBranchAsync(request.BranchId, cancellationToken);
        var dtos = tariffs.Select(x => new TariffDto(
            x.Id, x.BranchId, x.FirstHourRate, x.AdditionalHourRate, x.DailyMaxRate, x.IsActive)).ToList();
        return Result.Success(dtos);
    }
}
