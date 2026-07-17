namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Supplier.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllSuppliersByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithSuppliersInBranch_ShouldReturnDtos()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        var supplier = Supplier.Create(1, "Fornecedor Alpha", "12345678000190", "11999998888", "contato@alpha.com").Value;

        supplierRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Supplier> { supplier });

        var handler = new GetAllSuppliersByBranchQueryHandler(supplierRepository);
        var query = new GetAllSuppliersByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Name.Should().Be("Fornecedor Alpha");
    }

    [Fact]
    public async Task Handle_WithNoSuppliersInBranch_ShouldReturnEmptyList()
    {
        // Arrange
        var supplierRepository = Substitute.For<ISupplierRepository>();
        supplierRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Supplier>());

        var handler = new GetAllSuppliersByBranchQueryHandler(supplierRepository);
        var query = new GetAllSuppliersByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
