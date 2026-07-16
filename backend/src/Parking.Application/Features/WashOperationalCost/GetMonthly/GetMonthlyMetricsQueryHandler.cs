namespace Parking.Application.Features.WashOperationalCost.GetMonthly;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetMonthlyMetricsQueryHandler : IQueryHandler<GetMonthlyMetricsQuery, WashOperationalCostDto>
{
    private readonly IWashOperationalCostRepository _costRepository;

    public GetMonthlyMetricsQueryHandler(IWashOperationalCostRepository costRepository)
    {
        _costRepository = costRepository;
    }

    public async Task<Result<WashOperationalCostDto>> Handle(GetMonthlyMetricsQuery request, CancellationToken cancellationToken)
    {
        var cost = await _costRepository.GetByBranchAndMonthAsync(request.BranchId, request.MonthYear, cancellationToken);
        if (cost is null)
            return Result.Failure<WashOperationalCostDto>(
                new Error("WashOperationalCost.NotFound", "Monthly report not found."));

        return Result.Success(new WashOperationalCostDto(
            cost.Id,
            cost.BranchId,
            cost.MonthYear,
            cost.LaborCost,
            cost.MaterialCost,
            cost.EquipmentCost,
            cost.UtilitiesCost,
            cost.TotalCost,
            cost.TotalRevenue,
            cost.NetProfit,
            cost.ProfitMargin,
            cost.IsActive));
    }
}
