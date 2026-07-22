namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Product.GetStockLedger;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetStockLedgerQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingMovements_ShouldReturnStockMovementDtos()
    {
        // Arrange
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var movement = StockMovement.Create(1, StockMovement.CompraEntrada, 10m, 5m, "Compra inicial").Value;

        stockMovementRepository.GetByProductAsync(1, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<StockMovement> { movement });

        var handler = new GetStockLedgerQueryHandler(stockMovementRepository);

        var query = new GetStockLedgerQuery(1, null, null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].ProductId.Should().Be(1);
        result.Value[0].MovementType.Should().Be(StockMovement.CompraEntrada);
        result.Value[0].Quantity.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_WithNoMovements_ShouldReturnEmptyList()
    {
        // Arrange
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();

        stockMovementRepository.GetByProductAsync(1, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<StockMovement>());

        var handler = new GetStockLedgerQueryHandler(stockMovementRepository);

        var query = new GetStockLedgerQuery(1, null, null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithDateRange_ShouldPassDatesToRepository()
    {
        // Arrange
        var stockMovementRepository = Substitute.For<IStockMovementRepository>();
        var fromDate = new DateTime(2026, 1, 1);
        var toDate = new DateTime(2026, 1, 31);

        stockMovementRepository.GetByProductAsync(1, fromDate, toDate, Arg.Any<CancellationToken>())
            .Returns(new List<StockMovement>());

        var handler = new GetStockLedgerQueryHandler(stockMovementRepository);

        var query = new GetStockLedgerQuery(1, fromDate, toDate);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await stockMovementRepository.Received(1).GetByProductAsync(1, fromDate, toDate, Arg.Any<CancellationToken>());
    }
}
