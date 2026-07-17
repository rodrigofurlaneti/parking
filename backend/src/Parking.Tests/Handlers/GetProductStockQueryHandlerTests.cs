namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Product.GetStock;
using Parking.Domain.Repositories;
using Xunit;
using DomainProduct = Parking.Domain.Entities.Product;

public sealed class GetProductStockQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProducts_ShouldReturnProductDtos()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var product = DomainProduct.Create(1, "Cera Automotiva", "SKU-001", "Limpeza", 10m, 25m, 100m).Value;

        productRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<DomainProduct> { product });

        var handler = new GetProductStockQueryHandler(productRepository);

        var query = new GetProductStockQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].SKU.Should().Be("SKU-001");
        result.Value[0].Stock.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_WithNoProducts_ShouldReturnEmptyList()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();

        productRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<DomainProduct>());

        var handler = new GetProductStockQueryHandler(productRepository);

        var query = new GetProductStockQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
