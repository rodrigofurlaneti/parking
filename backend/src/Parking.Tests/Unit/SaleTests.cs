namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class SaleTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Sale.Create(1, 2, 3, 100, 50m);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.VehicleExitId.Should().Be(2);
        result.Value.CashRegisterId.Should().Be(3);
        result.Value.SaleNumber.Should().Be(100);
        result.Value.TotalAmount.Should().Be(50m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldFail()
    {
        var result = Sale.Create(1, 2, 3, 100, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Sale.InvalidAmount");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        var result = Sale.Create(1, 2, 3, 100, -10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Sale.InvalidAmount");
    }

    [Fact]
    public void Refund_WhenActive_ShouldSucceedAndDeactivate()
    {
        var sale = Sale.Create(1, 2, 3, 100, 50m).Value;

        var result = sale.Refund();

        result.IsSuccess.Should().BeTrue();
        sale.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Refund_WhenAlreadyRefunded_ShouldFail()
    {
        var sale = Sale.Create(1, 2, 3, 100, 50m).Value;
        sale.Refund();

        var result = sale.Refund();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Sale.AlreadyRefunded");
    }
}
