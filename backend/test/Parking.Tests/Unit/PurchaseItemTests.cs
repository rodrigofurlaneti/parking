namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class PurchaseItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = PurchaseItem.Create(1, 2, 10m, 5m);

        result.IsSuccess.Should().BeTrue();
        result.Value.PurchaseId.Should().Be(1);
        result.Value.ProductId.Should().Be(2);
        result.Value.QuantityOrdered.Should().Be(10m);
        result.Value.QuantityReceived.Should().Be(0m);
        result.Value.UnitCost.Should().Be(5m);
        result.Value.IsFullyReceived.Should().BeFalse();
    }

    [Fact]
    public void Create_WithInvalidPurchase_ShouldFail()
    {
        var result = PurchaseItem.Create(0, 2, 10m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.InvalidPurchase");
    }

    [Fact]
    public void Create_WithInvalidProduct_ShouldFail()
    {
        var result = PurchaseItem.Create(1, 0, 10m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.InvalidProduct");
    }

    [Fact]
    public void Create_WithZeroQuantityOrdered_ShouldFail()
    {
        var result = PurchaseItem.Create(1, 2, 0m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.InvalidQuantity");
    }

    [Fact]
    public void Create_WithNegativeUnitCost_ShouldFail()
    {
        var result = PurchaseItem.Create(1, 2, 10m, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.InvalidUnitCost");
    }

    [Fact]
    public void ReceiveQuantity_WithValidQuantity_ShouldIncreaseReceived()
    {
        var item = PurchaseItem.Create(1, 2, 10m, 5m).Value;

        var result = item.ReceiveQuantity(4m);

        result.IsSuccess.Should().BeTrue();
        item.QuantityReceived.Should().Be(4m);
        item.IsFullyReceived.Should().BeFalse();
    }

    [Fact]
    public void ReceiveQuantity_ExactlyOrdered_ShouldMarkFullyReceived()
    {
        var item = PurchaseItem.Create(1, 2, 10m, 5m).Value;

        item.ReceiveQuantity(10m);

        item.IsFullyReceived.Should().BeTrue();
    }

    [Fact]
    public void ReceiveQuantity_WithZeroQuantity_ShouldFail()
    {
        var item = PurchaseItem.Create(1, 2, 10m, 5m).Value;

        var result = item.ReceiveQuantity(0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.InvalidQuantity");
    }

    [Fact]
    public void ReceiveQuantity_ExceedingOrdered_ShouldFail()
    {
        var item = PurchaseItem.Create(1, 2, 10m, 5m).Value;

        var result = item.ReceiveQuantity(11m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.ExceedsOrdered");
    }

    [Fact]
    public void ReceiveQuantity_AccumulatingBeyondOrdered_ShouldFail()
    {
        var item = PurchaseItem.Create(1, 2, 10m, 5m).Value;
        item.ReceiveQuantity(8m);

        var result = item.ReceiveQuantity(3m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PurchaseItem.ExceedsOrdered");
    }
}
