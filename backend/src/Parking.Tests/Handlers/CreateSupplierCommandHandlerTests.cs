namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Supplier.CreateSupplier;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateSupplierCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateSupplier()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new CreateSupplierCommand(
            1, "Fornecedor Alpha", "12345678000190", "11999998888", "contato@alpha.com");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.Name.Should().Be("Fornecedor Alpha");
        result.Value.Document.Should().Be("12345678000190");
        result.Value.IsActive.Should().BeTrue();

        await supplierRepository.Received(1).AddAsync(
            Arg.Any<Domain.Entities.Supplier>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateSupplierCommandHandler(supplierRepository, unitOfWork);

        var command = new CreateSupplierCommand(1, string.Empty, "12345678000190", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidName");
    }
}
