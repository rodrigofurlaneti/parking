namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashSession.RecordProduct;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RecordProductConsumptionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldRecordConsumptionAndDecreaseStock()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var consumptionRepository = Substitute.For<IWashProductConsumptionRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var product = Product.Create(1, "Shampoo", "SKU1", "Limpeza", 5m, 10m, 20m).Value;
        productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new RecordProductConsumptionCommandHandler(
            productRepository, consumptionRepository, stockMovementRepository, unitOfWork);
        var command = new RecordProductConsumptionCommand(1, 1, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.QuantityUsed.Should().Be(5m);
        result.Value.UnitCost.Should().Be(5m);
        result.Value.TotalCost.Should().Be(25m);
        result.Value.RemainingStock.Should().Be(15m);
        await consumptionRepository.Received(1).AddAsync(Arg.Any<WashProductConsumption>(), Arg.Any<CancellationToken>());
        await stockMovementRepository.Received(1).AddAsync(Arg.Any<StockMovement>(), Arg.Any<CancellationToken>());
        await productRepository.Received(1).UpdateAsync(product, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithProductNotFound_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var consumptionRepository = Substitute.For<IWashProductConsumptionRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        productRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Product?)null);

        var handler = new RecordProductConsumptionCommandHandler(
            productRepository, consumptionRepository, stockMovementRepository, unitOfWork);
        var command = new RecordProductConsumptionCommand(1, 1, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.NotFound");
    }

    [Fact]
    public async Task Handle_WithInsufficientStock_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var consumptionRepository = Substitute.For<IWashProductConsumptionRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var product = Product.Create(1, "Shampoo", "SKU1", "Limpeza", 5m, 10m, 2m).Value;
        productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new RecordProductConsumptionCommandHandler(
            productRepository, consumptionRepository, stockMovementRepository, unitOfWork);
        var command = new RecordProductConsumptionCommand(1, product.Id, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InsufficientStock");
    }

    [Fact]
    public async Task Handle_WithZeroQuantity_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var consumptionRepository = Substitute.For<IWashProductConsumptionRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var product = Product.Create(1, "Shampoo", "SKU1", "Limpeza", 5m, 10m, 20m).Value;
        productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var handler = new RecordProductConsumptionCommandHandler(
            productRepository, consumptionRepository, stockMovementRepository, unitOfWork);
        var command = new RecordProductConsumptionCommand(1, product.Id, 0m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidQuantity");
    }
}
