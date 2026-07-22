namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Product.Create;
using Parking.Domain.Repositories;
using Xunit;
using DomainProduct = Parking.Domain.Entities.Product;

public sealed class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        productRepository.GetBySKUAsync(1, "SKU-001", Arg.Any<CancellationToken>()).Returns((DomainProduct?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateProductCommandHandler(productRepository, unitOfWork);

        var command = new CreateProductCommand(1, "Cera Automotiva", "SKU-001", "Limpeza", 10m, 25m, 100m, null, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.Name.Should().Be("Cera Automotiva");
        result.Value.SKU.Should().Be("SKU-001");
        result.Value.Stock.Should().Be(100m);
        result.Value.IsActive.Should().BeTrue();

        await productRepository.Received(1).AddAsync(Arg.Any<DomainProduct>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateSKU_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existing = DomainProduct.Create(1, "Existing", "SKU-001", "Limpeza", 5m, 10m, 10m).Value;
        productRepository.GetBySKUAsync(1, "SKU-001", Arg.Any<CancellationToken>()).Returns(existing);

        var handler = new CreateProductCommandHandler(productRepository, unitOfWork);

        var command = new CreateProductCommand(1, "Cera Automotiva", "SKU-001", "Limpeza", 10m, 25m, 100m, null, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.DuplicateSKU");
        await productRepository.DidNotReceive().AddAsync(Arg.Any<DomainProduct>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        productRepository.GetBySKUAsync(1, "SKU-001", Arg.Any<CancellationToken>()).Returns((DomainProduct?)null);

        var handler = new CreateProductCommandHandler(productRepository, unitOfWork);

        var command = new CreateProductCommand(1, string.Empty, "SKU-001", "Limpeza", 10m, 25m, 100m, null, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidName");
    }

    [Fact]
    public async Task Handle_WithNegativeCost_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        productRepository.GetBySKUAsync(1, "SKU-001", Arg.Any<CancellationToken>()).Returns((DomainProduct?)null);

        var handler = new CreateProductCommandHandler(productRepository, unitOfWork);

        var command = new CreateProductCommand(1, "Cera Automotiva", "SKU-001", "Limpeza", -1m, 25m, 100m, null, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidCost");
    }

    [Fact]
    public async Task Handle_WithNegativeStock_ShouldFail()
    {
        // Arrange
        var productRepository = Substitute.For<IProductRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        productRepository.GetBySKUAsync(1, "SKU-001", Arg.Any<CancellationToken>()).Returns((DomainProduct?)null);

        var handler = new CreateProductCommandHandler(productRepository, unitOfWork);

        var command = new CreateProductCommand(1, "Cera Automotiva", "SKU-001", "Limpeza", 10m, 25m, -5m, null, 5m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidStock");
    }
}
