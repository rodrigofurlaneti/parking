namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Supplier.UpdateSupplier;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateSupplierCommandHandlerTests
{
    private static Supplier CreateSupplier() =>
        Supplier.Create(1, "Fornecedor Alpha", "12345678000190", "11999998888", "contato@alpha.com").Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateSupplier()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var supplier = CreateSupplier();

        supplierRepository.GetByIdAsync(supplier.Id, Arg.Any<CancellationToken>()).Returns(supplier);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new UpdateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new UpdateSupplierCommand(
            supplier.Id, "Fornecedor Beta", "98765432000110", "11977776666", "novo@beta.com");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Fornecedor Beta");
        result.Value.Document.Should().Be("98765432000110");
        result.Value.Phone.Should().Be("11977776666");
        result.Value.Email.Should().Be("novo@beta.com");

        await supplierRepository.Received(1).UpdateAsync(Arg.Any<Supplier>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithSupplierNotFound_ShouldFail()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        supplierRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Supplier?)null);

        var handler = new UpdateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new UpdateSupplierCommand(999, "Fornecedor Beta", "98765432000110", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var supplier = CreateSupplier();

        supplierRepository.GetByIdAsync(supplier.Id, Arg.Any<CancellationToken>()).Returns(supplier);

        var handler = new UpdateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new UpdateSupplierCommand(supplier.Id, string.Empty, "98765432000110", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidName");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyDocument_ShouldFail()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var supplier = CreateSupplier();

        supplierRepository.GetByIdAsync(supplier.Id, Arg.Any<CancellationToken>()).Returns(supplier);

        var handler = new UpdateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new UpdateSupplierCommand(supplier.Id, "Fornecedor Beta", string.Empty, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidDocument");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
