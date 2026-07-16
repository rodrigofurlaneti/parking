namespace Parking.Application.Features.Reports.GetCashReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class GetCashReportQueryHandler : IQueryHandler<GetCashReportQuery, CashReportDto>
{
    private const int MovementTypeEntry = 1;
    private const int MovementTypeExit = 2;
    private const int MovementTypeAdjustment = 3;

    private readonly IReportsRepository _reportsRepository;

    public GetCashReportQueryHandler(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<CashReportDto>> Handle(GetCashReportQuery request, CancellationToken cancellationToken)
    {
        var cashRegisters = await _reportsRepository.GetClosedCashRegistersAsync(
            request.BranchId, request.FromDate, request.ToDate, cancellationToken);

        var cashRegisterIds = cashRegisters.Select(x => x.Id).ToList();
        var movements = await _reportsRepository.GetCashMovementsAsync(cashRegisterIds, cancellationToken);

        var movementsByRegister = movements
            .GroupBy(m => m.CashRegisterId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var reconciliations = cashRegisters
            .Select(register => BuildReconciliation(register, movementsByRegister))
            .ToList();

        var operatorSummary = reconciliations
            .GroupBy(r => r.EmployeeId)
            .Select(g => new OperatorSummaryDto(g.Key, g.Count(), g.Sum(r => r.Difference)))
            .ToList();

        var dto = new CashReportDto(
            request.BranchId,
            request.FromDate,
            request.ToDate,
            reconciliations,
            operatorSummary);

        return Result.Success(dto);
    }

    private static CashRegisterReconciliationDto BuildReconciliation(
        CashRegister register,
        IReadOnlyDictionary<long, List<CashMovement>> movementsByRegister)
    {
        var registerMovements = movementsByRegister.TryGetValue(register.Id, out var list)
            ? list
            : new List<CashMovement>();

        var totalEntries = registerMovements.Where(m => m.Type == MovementTypeEntry).Sum(m => m.Amount);
        var totalExits = registerMovements.Where(m => m.Type == MovementTypeExit).Sum(m => m.Amount);

        // CashMovement.Amount for Type=Adjustment can be positive or negative (see CashMovement.Create),
        // so a plain sum already nets out correctly: a positive adjustment increases the expected
        // balance, a negative adjustment decreases it. No conditional logic is needed here.
        var totalAdjustments = registerMovements.Where(m => m.Type == MovementTypeAdjustment).Sum(m => m.Amount);

        var expectedAmount = register.OpeningBalance + totalEntries - totalExits + totalAdjustments;
        var difference = register.ClosingBalance - expectedAmount;

        return new CashRegisterReconciliationDto(
            register.Id,
            register.EmployeeId,
            register.OpenedAt,
            register.ClosedAt,
            expectedAmount,
            register.ClosingBalance,
            difference);
    }
}
