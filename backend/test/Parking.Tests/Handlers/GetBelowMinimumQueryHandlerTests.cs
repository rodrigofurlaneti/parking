namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Product.GetBelowMinimum;
using Parking.Domain.Repositories;
using Xunit;
using DomainProduct = Parking.Domain.Entities.Product;

public sealed class GetBelowMinimumQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithProductsBelowMinimum_ShouldReturnOnlyThose()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var belowMinimum = DomainProduct.Create(1, "Cera Automotiva", "SKU-001", "Limpeza", 10m, 25m, 2m, null, 5m).Value;
        var aboveMinimum = DomainProduct.Create(1, "Shampoo Automotivo", "SKU-002", "Limpeza", 8m, 20m, 50m, null, 5m).Value;

        productRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<DomainProduct> { belowMinimum, aboveMinimum });

        var handler = new GetBelowMinimumQueryHandler(productRepository);

        var query = new GetBelowMinimumQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].SKU.Should().Be("SKU-001");
    }

    [Fact]
    public async Task Handle_WithNoProductsBelowMinimum_ShouldReturnEmptyList()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var aboveMinimum = DomainProduct.Create(1, "Shampoo Automotivo", "SKU-002", "Limpeza", 8m, 20m, 50m, null, 5m).Value;

        productRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<DomainProduct> { aboveMinimum });

        var handler = new GetBelowMinimumQueryHandler(productRepository);

        var query = new GetBelowMinimumQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
