namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class StockMovementTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = StockMovement.Create(1, StockMovement.CompraEntrada, 10m, 2.5m, "Purchase received");

        result.IsSuccess.Should().BeTrue();
        result.Value.ProductId.Should().Be(1);
        result.Value.MovementType.Should().Be(StockMovement.CompraEntrada);
        result.Value.Quantity.Should().Be(10m);
        result.Value.UnitCost.Should().Be(2.5m);
        result.Value.Reason.Should().Be("Purchase received");
    }

    [Fact]
    public void Create_WithOptionalReferencedDocument_ShouldSetValues()
    {
        var result = StockMovement.Create(1, StockMovement.CompraEntrada, 10m, 2.5m, "Purchase", "Purchase", 99);

        result.IsSuccess.Should().BeTrue();
        result.Value.ReferencedDocumentType.Should().Be("Purchase");
        result.Value.ReferencedDocumentId.Should().Be(99);
    }

    [Fact]
    public void Create_WithInvalidProduct_ShouldFail()
    {
        var result = StockMovement.Create(0, StockMovement.CompraEntrada, 10m, 2.5m, "reason");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StockMovement.InvalidProduct");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Create_WithInvalidType_ShouldFail(int type)
    {
        var result = StockMovement.Create(1, type, 10m, 2.5m, "reason");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StockMovement.InvalidType");
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldFail()
    {
        var result = StockMovement.Create(1, StockMovement.CompraEntrada, 0m, 2.5m, "reason");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StockMovement.InvalidQuantity");
    }

    [Fact]
    public void Create_WithNegativeUnitCost_ShouldFail()
    {
        var result = StockMovement.Create(1, StockMovement.CompraEntrada, 10m, -1m, "reason");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StockMovement.InvalidUnitCost");
    }

    [Fact]
    public void Create_WithMissingReason_ShouldFail()
    {
        var result = StockMovement.Create(1, StockMovement.CompraEntrada, 10m, 2.5m, "  ");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StockMovement.InvalidReason");
    }

    [Theory]
    [InlineData(StockMovement.CompraEntrada, true)]
    [InlineData(StockMovement.AjustePositivo, true)]
    [InlineData(StockMovement.ConsumoSaida, false)]
    [InlineData(StockMovement.AjusteNegativo, false)]
    public void IsInflow_ShouldReturnExpected(int movementType, bool expected)
    {
        var movement = StockMovement.Create(1, movementType, 10m, 2.5m, "reason").Value;

        movement.IsInflow().Should().Be(expected);
    }
}
