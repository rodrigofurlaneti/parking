namespace Parking.Application.Features.WashOperationalCost.GenerateReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainWashOperationalCost = Parking.Domain.Entities.WashOperationalCost;

internal sealed class GenerateMonthlyReportCommandHandler
    : ICommandHandler<GenerateMonthlyReportCommand, WashOperationalCostDto>
{
    private readonly IWashOperationalCostRepository _costRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateMonthlyReportCommandHandler(
        IWashOperationalCostRepository costRepository,
        IUnitOfWork unitOfWork)
    {
        _costRepository = costRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WashOperationalCostDto>> Handle(
        GenerateMonthlyReportCommand request, CancellationToken cancellationToken)
    {
        var existing = await _costRepository.GetByBranchAndMonthAsync(request.BranchId, request.MonthYear, cancellationToken);
        if (existing is not null)
            return Result.Failure<WashOperationalCostDto>(
                new Error("WashOperationalCost.DuplicateReport", "A report for this branch and month already exists."));

        var costResult = DomainWashOperationalCost.Create(
            request.BranchId,
            request.MonthYear,
            request.LaborCost,
            request.MaterialCost,
            request.EquipmentCost,
            request.UtilitiesCost,
            request.TotalRevenue);

        if (costResult.IsFailure)
            return Result.Failure<WashOperationalCostDto>(costResult.Error);

        await _costRepository.AddAsync(costResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new WashOperationalCostDto(
            costResult.Value.Id,
            costResult.Value.BranchId,
            costResult.Value.MonthYear,
            costResult.Value.LaborCost,
            costResult.Value.MaterialCost,
            costResult.Value.EquipmentCost,
            costResult.Value.UtilitiesCost,
            costResult.Value.TotalCost,
            costResult.Value.TotalRevenue,
            costResult.Value.NetProfit,
            costResult.Value.ProfitMargin,
            costResult.Value.IsActive));
    }
}
