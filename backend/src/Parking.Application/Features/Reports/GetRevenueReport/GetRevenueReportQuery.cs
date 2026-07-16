namespace Parking.Application.Features.Reports.GetRevenueReport;

using Parking.Application.Abstractions.Messaging;

public sealed record GetRevenueReportQuery(long BranchId, DateTime FromDate, DateTime ToDate) : IQuery<RevenueReportDto>;
