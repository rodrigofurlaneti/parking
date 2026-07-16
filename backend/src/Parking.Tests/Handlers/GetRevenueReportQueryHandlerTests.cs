namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Reports.GetRevenueReport;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetRevenueReportQueryHandlerTests
{
    private static Sale CreateSale(decimal amount) =>
        Sale.Create(1, 1, 1, 1, amount).Value;

    [Fact]
    public async Task Handle_WithSalesAndWashRevenue_ShouldAggregateByCustomerType()
    {
        // Arrange
        var reportsRepository = Substitute.For<IReportsRepository>();
        var fromDate = new DateTime(2026, 7, 1);
        var toDate = new DateTime(2026, 7, 31);

        var sales = new List<SaleWithCustomerType>
        {
            new(CreateSale(100m), 1), // Rotativo
            new(CreateSale(50m), 1),  // Rotativo
            new(CreateSale(200m), 2), // Convenio
            new(CreateSale(300m), 3), // Mensalista
        };

        reportsRepository.GetSalesWithCustomerTypeAsync(1, fromDate, toDate, Arg.Any<CancellationToken>())
            .Returns(sales);
        reportsRepository.GetWashServiceRevenueTotalAsync(1, fromDate, toDate, Arg.Any<CancellationToken>())
            .Returns(150m);

        var handler = new GetRevenueReportQueryHandler(reportsRepository);
        var query = new GetRevenueReportQuery(1, fromDate, toDate);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RotativeRevenue.Should().Be(150m);
        result.Value.AgreementRevenue.Should().Be(200m);
        result.Value.MonthlyRevenue.Should().Be(300m);
        result.Value.ParkingTotalRevenue.Should().Be(650m);
        result.Value.WashServiceRevenue.Should().Be(150m);
        result.Value.GrandTotal.Should().Be(800m);
    }
}
