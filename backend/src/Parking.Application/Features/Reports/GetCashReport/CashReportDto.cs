namespace Parking.Application.Features.Reports.GetCashReport;

public sealed record CashReportDto(
    long BranchId,
    DateTime FromDate,
    DateTime ToDate,
    IReadOnlyCollection<CashRegisterReconciliationDto> Reconciliations,
    IReadOnlyCollection<OperatorSummaryDto> OperatorSummary);

public sealed record CashRegisterReconciliationDto(
    long CashRegisterId,
    long EmployeeId,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal ExpectedAmount,
    decimal ClosingBalance,
    decimal Difference);

public sealed record OperatorSummaryDto(long EmployeeId, int RegistersOperated, decimal TotalDifference);
