namespace Parking.Tests.Handlers;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Purchase.CreatePurchase;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreatePurchaseCommandHandlerTests
{
    private static readonly PropertyInfo IdProperty = typeof(Entity).GetProperty(nameof(Entity.Id))!;

    private static Supplier CreateSupplier() =>
        Supplier.Create(1, "Fornecedor Alpha", "12345678000190", null, null).Value;

    [Fact]
    public async Task Handle_WithNonExistingSupplier_ShouldFail()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var purchaseRepository = Substitute.For<IPurchaseRepository>();
        var purchaseItemRepository = Substitute.For<IPurchaseItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        supplierRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Supplier?)null);

        var handler = new CreatePurchaseCommandHandler(
            supplierRepository, purchaseRepository, purchaseItemRepository, unitOfWork);

        var command = new CreatePurchaseCommand(
            1, 99, new List<PurchaseItemInput> { new(1, 10m, 5m) });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.NotFound");
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreatePurchaseWithItems()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var purchaseRepository = Substitute.For<IPurchaseRepository>();
        var purchaseItemRepository = Substitute.For<IPurchaseItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var supplier = CreateSupplier();

        supplierRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(supplier);
        purchaseRepository.GetNextPurchaseNumberAsync(1, Arg.Any<CancellationToken>()).Returns(1L);

        // Simulate EF assigning the DB-generated identity to the Purchase entity
        // the first time the pending insert is flushed via CommitAsync.
        Purchase? trackedPurchase = null;
        purchaseRepository
            .When(x => x.AddAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => trackedPurchase = callInfo.Arg<Purchase>());

        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            if (trackedPurchase is not null && trackedPurchase.Id == 0)
                IdProperty.SetValue(trackedPurchase, 1L);

            return 1;
        });

        var handler = new CreatePurchaseCommandHandler(
            supplierRepository, purchaseRepository, purchaseItemRepository, unitOfWork);

        var command = new CreatePurchaseCommand(
            1,
            1,
            new List<PurchaseItemInput>
            {
                new(10, 5m, 2m),
                new(20, 3m, 4m),
            });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.SupplierId.Should().Be(1);
        result.Value.PurchaseNumber.Should().Be(1);
        result.Value.Status.Should().Be(Purchase.Pending);
        result.Value.Items.Should().HaveCount(2);

        await purchaseRepository.Received(1).AddAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
        await purchaseItemRepository.Received(2).AddAsync(Arg.Any<PurchaseItem>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(2).CommitAsync(Arg.Any<CancellationToken>());
    }
}
