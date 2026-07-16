namespace Parking.Tests.Handlers;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Reports.GetOccupancyReport;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetOccupancyReportQueryHandlerTests
{
    private static void SetProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        property.SetValue(obj, value);
    }

    private static VehicleEntry CreateEntry(DateTime entryTime)
    {
        var entry = VehicleEntry.Create(1, 1, 1, "ABC1234", "Model", "Black").Value;
        SetProperty(entry, nameof(VehicleEntry.EntryTime), entryTime);
        return entry;
    }

    private static VehicleExit CreateExit(int durationMinutes) =>
        VehicleExit.Create(1, durationMinutes, 10m, 1).Value;

    [Fact]
    public async Task Handle_WithEntriesAndExits_ShouldComputeAggregatesCorrectly()
    {
        // Arrange
        var reportsRepository = Substitute.For<IReportsRepository>();

        var fromDate = new DateTime(2026, 7, 1, 0, 0, 0);
        var toDate = new DateTime(2026, 7, 2, 0, 0, 0); // 1440 minutes period

        var entries = new List<VehicleEntry>
        {
            CreateEntry(new DateTime(2026, 7, 1, 8, 0, 0)),
            CreateEntry(new DateTime(2026, 7, 1, 8, 30, 0)),
            CreateEntry(new DateTime(2026, 7, 1, 10, 0, 0)),
        };

        var exits = new List<VehicleExit>
        {
            CreateExit(60),
            CreateExit(120),
            CreateExit(180),
        };

        reportsRepository.CountActiveParkingSpacesAsync(1, Arg.Any<CancellationToken>()).Returns(2);
        reportsRepository.GetVehicleEntriesAsync(1, fromDate, toDate, Arg.Any<CancellationToken>()).Returns(entries);
        reportsRepository.GetVehicleExitsAsync(1, fromDate, toDate, Arg.Any<CancellationToken>()).Returns(exits);

        var handler = new GetOccupancyReportQueryHandler(reportsRepository);
        var query = new GetOccupancyReportQuery(1, fromDate, toDate);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalSpaces.Should().Be(2);
        result.Value.TotalEntries.Should().Be(3);
        result.Value.PeakHour.Should().Be(8);
        // 360 total occupied minutes / (2 spaces * 1440 minutes) * 100 = 12.5
        result.Value.AverageOccupancyPercentage.Should().Be(12.5m);
        result.Value.HourlyBreakdown.Should().HaveCount(24);
        result.Value.HourlyBreakdown.Single(h => h.Hour == 8).EntryCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithNoSpaces_ShouldReturnZeroOccupancy()
    {
        // Arrange
        var reportsRepository = Substitute.For<IReportsRepository>();
        var fromDate = new DateTime(2026, 7, 1);
        var toDate = new DateTime(2026, 7, 2);

        reportsRepository.CountActiveParkingSpacesAsync(1, Arg.Any<CancellationToken>()).Returns(0);
        reportsRepository.GetVehicleEntriesAsync(1, fromDate, toDate, Arg.Any<CancellationToken>()).Returns(new List<VehicleEntry>());
        reportsRepository.GetVehicleExitsAsync(1, fromDate, toDate, Arg.Any<CancellationToken>()).Returns(new List<VehicleExit>());

        var handler = new GetOccupancyReportQueryHandler(reportsRepository);
        var query = new GetOccupancyReportQuery(1, fromDate, toDate);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AverageOccupancyPercentage.Should().Be(0m);
        result.Value.TotalEntries.Should().Be(0);
    }
}
