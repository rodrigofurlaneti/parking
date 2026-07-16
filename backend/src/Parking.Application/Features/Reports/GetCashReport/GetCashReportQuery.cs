namespace Parking.Application.Features.Reports.GetCashReport;

using Parking.Application.Abstractions.Messaging;

public sealed record GetCashReportQuery(long BranchId, DateTime FromDate, DateTime ToDate) : IQuery<CashReportDto>;
