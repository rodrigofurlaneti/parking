namespace Parking.Application.Features.Reports.GetOccupancyReport;

public sealed record OccupancyReportDto(
    long BranchId,
    DateTime FromDate,
    DateTime ToDate,
    int TotalSpaces,
    int TotalEntries,
    decimal AverageOccupancyPercentage,
    int PeakHour,
    IReadOnlyCollection<HourlyOccupancyDto> HourlyBreakdown);

public sealed record HourlyOccupancyDto(int Hour, int EntryCount);
