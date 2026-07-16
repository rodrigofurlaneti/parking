namespace Parking.Tests.Handlers;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Reports.GetStockReport;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetStockReportQueryHandlerTests
{
    private static readonly PropertyInfo IdProperty = typeof(Entity).GetProperty(nameof(Entity.Id))!;

    private static Product CreateProduct(long id, string name, decimal stock, decimal minimumStock, decimal cost)
    {
        var product = Product.Create(1, name, $"SKU-{id}", "Categoria", cost, cost * 2, stock, null, minimumStock).Value;
        IdProperty.SetValue(product, id);
        return product;
    }

    private static StockMovement CreateMovement(long productId) =>
        StockMovement.Create(productId, StockMovement.CompraEntrada, 1m, 5m, "Compra").Value;

    [Fact]
    public async Task Handle_WithProductsAndMovements_ShouldComputeAggregatesCorrectly()
    {
        // Arrange
        var reportsRepository = Substitute.For<IReportsRepository>();

        var product1 = CreateProduct(1, "Shampoo", 5m, 10m, 2m);  // below minimum, value = 10
        var product2 = CreateProduct(2, "Cera", 20m, 5m, 3m);     // ok, value = 60
        var product3 = CreateProduct(3, "Pano", 0m, 0m, 1m);      // not below (0 < 0 is false), value = 0

        var products = new List<Product> { product1, product2, product3 };

        var movements = new List<StockMovement>
        {
            CreateMovement(2), CreateMovement(2), CreateMovement(2),
            CreateMovement(1), CreateMovement(1),
            CreateMovement(3),
        };

        reportsRepository.GetActiveProductsAsync(1, Arg.Any<CancellationToken>()).Returns(products);
        reportsRepository.GetRecentStockMovementsAsync(1, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(movements);

        var handler = new GetStockReportQueryHandler(reportsRepository);
        var query = new GetStockReportQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalStockValue.Should().Be(70m);
        result.Value.BelowMinimumProducts.Should().ContainSingle(p => p.Id == 1);
        result.Value.TopMovedProducts.Should().HaveCount(3);
        result.Value.TopMovedProducts.First().ProductId.Should().Be(2);
        result.Value.TopMovedProducts.First().MovementCount.Should().Be(3);
        result.Value.TopMovedProducts.First().ProductName.Should().Be("Cera");
    }
}
