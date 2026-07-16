namespace Parking.Application.Features.Reports.GetOccupancyReport;

using Parking.Application.Abstractions.Messaging;

public sealed record GetOccupancyReportQuery(long BranchId, DateTime FromDate, DateTime ToDate) : IQuery<OccupancyReportDto>;
