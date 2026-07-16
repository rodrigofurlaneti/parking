namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Product.AdjustStock;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class AdjustStockCommandHandlerTests
{
    private const long ProductId = 1;

    private static Product CreateProduct(decimal stock) =>
        Product.Create(1, "Produto X", "SKU-X", "Categoria", 3m, 10m, stock).Value;

    [Fact]
    public async Task Handle_WithPositiveAdjustment_ShouldIncreaseStock()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var product = CreateProduct(10m);

        productRepository.GetByIdAsync(ProductId, Arg.Any<CancellationToken>()).Returns(product);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new AdjustStockCommandHandler(productRepository, stockMovementRepository, unitOfWork);

        var command = new AdjustStockCommand(ProductId, 5m, "Contagem de inventario");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Stock.Should().Be(15m);

        await stockMovementRepository.Received(1).AddAsync(
            Arg.Is<StockMovement>(m => m.MovementType == StockMovement.AjustePositivo && m.Quantity == 5m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNegativeAdjustmentExceedingStock_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var product = CreateProduct(10m);

        productRepository.GetByIdAsync(ProductId, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new AdjustStockCommandHandler(productRepository, stockMovementRepository, unitOfWork);

        var command = new AdjustStockCommand(ProductId, -20m, "Perda de inventario");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InsufficientStock");

        await stockMovementRepository.DidNotReceive().AddAsync(
            Arg.Any<StockMovement>(), Arg.Any<CancellationToken>());
    }
}
