namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashOperationalCost.GetMonthly;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetMonthlyMetricsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingReport_ShouldReturnMetrics()
    {
        // Arrange
        var costRepository = Substitute.For<IWashOperationalCostRepository>();

        var existing = WashOperationalCost.Create(1, new DateTime(2026, 7, 1), 450m, 300m, 100m, 50m, 2250m).Value;
        costRepository.GetByBranchAndMonthAsync(1, new DateTime(2026, 7, 1), Arg.Any<CancellationToken>())
            .Returns(existing);

        var handler = new GetMonthlyMetricsQueryHandler(costRepository);
        var query = new GetMonthlyMetricsQuery(1, new DateTime(2026, 7, 1));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(2250m);
        result.Value.TotalCost.Should().Be(900m);
        result.Value.NetProfit.Should().Be(1350m);
    }

    [Fact]
    public async Task Handle_WithNoReport_ShouldFail()
    {
        // Arrange
        var costRepository = Substitute.For<IWashOperationalCostRepository>();

        costRepository.GetByBranchAndMonthAsync(Arg.Any<long>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((WashOperationalCost?)null);

        var handler = new GetMonthlyMetricsQueryHandler(costRepository);
        var query = new GetMonthlyMetricsQuery(1, new DateTime(2026, 7, 1));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashOperationalCost.NotFound");
    }
}
