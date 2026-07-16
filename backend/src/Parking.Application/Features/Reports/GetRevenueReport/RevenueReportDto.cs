namespace Parking.Application.Features.Reports.GetRevenueReport;

public sealed record RevenueReportDto(
    long BranchId,
    DateTime FromDate,
    DateTime ToDate,
    decimal RotativeRevenue,
    decimal AgreementRevenue,
    decimal MonthlyRevenue,
    decimal ParkingTotalRevenue,
    decimal WashServiceRevenue,
    decimal GrandTotal);
