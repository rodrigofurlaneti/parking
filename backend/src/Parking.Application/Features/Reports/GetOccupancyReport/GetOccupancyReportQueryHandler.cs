namespace Parking.Application.Features.Reports.GetOccupancyReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetOccupancyReportQueryHandler : IQueryHandler<GetOccupancyReportQuery, OccupancyReportDto>
{
    private readonly IReportsRepository _reportsRepository;

    public GetOccupancyReportQueryHandler(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<OccupancyReportDto>> Handle(GetOccupancyReportQuery request, CancellationToken cancellationToken)
    {
        var totalSpaces = await _reportsRepository.CountActiveParkingSpacesAsync(request.BranchId, cancellationToken);
        var entries = await _reportsRepository.GetVehicleEntriesAsync(request.BranchId, request.FromDate, request.ToDate, cancellationToken);
        var exits = await _reportsRepository.GetVehicleExitsAsync(request.BranchId, request.FromDate, request.ToDate, cancellationToken);

        var hourlyBreakdown = Enumerable.Range(0, 24)
            .Select(hour => new HourlyOccupancyDto(hour, entries.Count(e => e.EntryTime.Hour == hour)))
            .ToList();

        var peakHour = hourlyBreakdown.Count > 0
            ? hourlyBreakdown.OrderByDescending(h => h.EntryCount).ThenBy(h => h.Hour).First().Hour
            : 0;

        var totalOccupiedMinutes = exits.Sum(x => (decimal)x.DurationMinutes);
        var periodMinutes = (decimal)Math.Max((request.ToDate - request.FromDate).TotalMinutes, 0);

        decimal averageOccupancyPercentage = 0m;
        if (totalSpaces > 0 && periodMinutes > 0)
        {
            averageOccupancyPercentage = Math.Round(
                totalOccupiedMinutes / (totalSpaces * periodMinutes) * 100m,
                2);
        }

        var dto = new OccupancyReportDto(
            request.BranchId,
            request.FromDate,
            request.ToDate,
            totalSpaces,
            entries.Count,
            averageOccupancyPercentage,
            peakHour,
            hourlyBreakdown);

        return Result.Success(dto);
    }
}
