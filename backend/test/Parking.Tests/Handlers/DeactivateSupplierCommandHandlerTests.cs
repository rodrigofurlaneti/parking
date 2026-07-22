namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Supplier.DeactivateSupplier;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class DeactivateSupplierCommandHandlerTests
{
    private static Supplier CreateSupplier() =>
        Supplier.Create(1, "Fornecedor Alpha", "12345678000190", "11999998888", "contato@alpha.com").Value;

    [Fact]
    public async Task Handle_WithValidSupplier_ShouldDeactivate()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var supplier = CreateSupplier();

        supplierRepository.GetByIdAsync(supplier.Id, Arg.Any<CancellationToken>()).Returns(supplier);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new DeactivateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new DeactivateSupplierCommand(supplier.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        supplier.IsActive.Should().BeFalse();

        await supplierRepository.Received(1).UpdateAsync(supplier, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithSupplierNotFound_ShouldFail()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        supplierRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Supplier?)null);

        var handler = new DeactivateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new DeactivateSupplierCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
