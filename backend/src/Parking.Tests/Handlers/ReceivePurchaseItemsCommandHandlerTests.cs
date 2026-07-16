namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Purchase.ReceivePurchaseItems;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class ReceivePurchaseItemsCommandHandlerTests
{
    private const long PurchaseId = 1;

    private static Purchase CreatePurchase() =>
        Purchase.Create(1, 1, 1).Value;

    private static PurchaseItem CreatePurchaseItem(decimal quantityOrdered) =>
        PurchaseItem.Create(PurchaseId, 50, quantityOrdered, 3m).Value;

    private static Product CreateProduct(decimal stock) =>
        Product.Create(1, "Produto X", "SKU-X", "Categoria", 3m, 10m, stock).Value;

    [Fact]
    public async Task Handle_WithQuantityGreaterThanOrdered_ShouldFail()
    {
        // Arrange
        var purchaseRepository = Substitute.For<IPurchaseRepository>();
        var purchaseItemRepository = Substitute.For<IPurchaseItemRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var purchase = CreatePurchase();
        var purchaseItem = CreatePurchaseItem(5m);

        purchaseRepository.GetByIdAsync(PurchaseId, Arg.Any<CancellationToken>()).Returns(purchase);
        purchaseItemRepository.GetByIdAsync(purchaseItem.Id, Arg.Any<CancellationToken>()).Returns(purchaseItem);

        var handler = new ReceivePurchaseItemsCommandHandler(
            purchaseRepository, purchaseItemRepository, productRepository, stockMovementRepository, unitOfWork);

        var command = new ReceivePurchaseItemsCommand(
            PurchaseId, new List<ReceivePurchaseItemInput> { new(purchaseItem.Id, 10m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.ExceedsOrdered");
    }

    [Fact]
    public async Task Handle_WithPartialReceipt_ShouldMarkPurchaseAsPartiallyReceived()
    {
        // Arrange
        var purchaseRepository = Substitute.For<IPurchaseRepository>();
        var purchaseItemRepository = Substitute.For<IPurchaseItemRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var purchase = CreatePurchase();
        var purchaseItem = CreatePurchaseItem(10m);
        var product = CreateProduct(20m);

        purchaseRepository.GetByIdAsync(PurchaseId, Arg.Any<CancellationToken>()).Returns(purchase);
        purchaseItemRepository.GetByIdAsync(purchaseItem.Id, Arg.Any<CancellationToken>()).Returns(purchaseItem);
        productRepository.GetByIdAsync(50, Arg.Any<CancellationToken>()).Returns(product);
        purchaseItemRepository.GetByPurchaseAsync(PurchaseId, Arg.Any<CancellationToken>())
            .Returns(new List<PurchaseItem> { purchaseItem });
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new ReceivePurchaseItemsCommandHandler(
            purchaseRepository, purchaseItemRepository, productRepository, stockMovementRepository, unitOfWork);

        var command = new ReceivePurchaseItemsCommand(
            PurchaseId, new List<ReceivePurchaseItemInput> { new(purchaseItem.Id, 4m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Purchase.PartiallyReceived);
        purchaseItem.QuantityReceived.Should().Be(4m);
        purchaseItem.IsFullyReceived.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithFullReceipt_ShouldMarkPurchaseAsReceivedAndIncreaseProductStock()
    {
        // Arrange
        var purchaseRepository = Substitute.For<IPurchaseRepository>();
        var purchaseItemRepository = Substitute.For<IPurchaseItemRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var purchase = CreatePurchase();
        var purchaseItem = CreatePurchaseItem(5m);
        var product = CreateProduct(20m);

        purchaseRepository.GetByIdAsync(PurchaseId, Arg.Any<CancellationToken>()).Returns(purchase);
        purchaseItemRepository.GetByIdAsync(purchaseItem.Id, Arg.Any<CancellationToken>()).Returns(purchaseItem);
        productRepository.GetByIdAsync(50, Arg.Any<CancellationToken>()).Returns(product);
        purchaseItemRepository.GetByPurchaseAsync(PurchaseId, Arg.Any<CancellationToken>())
            .Returns(new List<PurchaseItem> { purchaseItem });
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new ReceivePurchaseItemsCommandHandler(
            purchaseRepository, purchaseItemRepository, productRepository, stockMovementRepository, unitOfWork);

        var command = new ReceivePurchaseItemsCommand(
            PurchaseId, new List<ReceivePurchaseItemInput> { new(purchaseItem.Id, 5m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Purchase.Received);
        purchaseItem.IsFullyReceived.Should().BeTrue();
        product.Stock.Should().Be(25m);

        await stockMovementRepository.Received(1).AddAsync(
            Arg.Is<StockMovement>(m => m.MovementType == StockMovement.CompraEntrada && m.Quantity == 5m),
            Arg.Any<CancellationToken>());
    }
}
