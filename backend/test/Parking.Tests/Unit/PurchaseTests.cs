namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class PurchaseTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Purchase.Create(1, 2, 100);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.SupplierId.Should().Be(2);
        result.Value.PurchaseNumber.Should().Be(100);
        result.Value.Status.Should().Be(Purchase.Pending);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidBranch_ShouldFail()
    {
        var result = Purchase.Create(0, 2, 100);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Purchase.InvalidBranch");
    }

    [Fact]
    public void Create_WithInvalidSupplier_ShouldFail()
    {
        var result = Purchase.Create(1, 0, 100);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Purchase.InvalidSupplier");
    }

    [Fact]
    public void Create_WithInvalidPurchaseNumber_ShouldFail()
    {
        var result = Purchase.Create(1, 2, 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Purchase.InvalidNumber");
    }

    [Fact]
    public void MarkAsReceived_ShouldSetStatusReceived()
    {
        var purchase = Purchase.Create(1, 2, 100).Value;

        purchase.MarkAsReceived();

        purchase.Status.Should().Be(Purchase.Received);
    }

    [Fact]
    public void MarkAsPartiallyReceived_ShouldSetStatusPartiallyReceived()
    {
        var purchase = Purchase.Create(1, 2, 100).Value;

        purchase.MarkAsPartiallyReceived();

        purchase.Status.Should().Be(Purchase.PartiallyReceived);
    }
}
