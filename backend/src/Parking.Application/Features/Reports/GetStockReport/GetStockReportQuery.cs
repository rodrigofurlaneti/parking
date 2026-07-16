namespace Parking.Application.Features.Reports.GetStockReport;

using Parking.Application.Abstractions.Messaging;

public sealed record GetStockReportQuery(long BranchId) : IQuery<StockReportDto>;
