namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class SalePaymentTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = SalePayment.Create(1, 1, 25m);

        result.IsSuccess.Should().BeTrue();
        result.Value.SaleId.Should().Be(1);
        result.Value.PaymentMethod.Should().Be(1);
        result.Value.Amount.Should().Be(25m);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldFail()
    {
        var result = SalePayment.Create(1, 1, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("SalePayment.InvalidAmount");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        var result = SalePayment.Create(1, 1, -5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("SalePayment.InvalidAmount");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_WithInvalidPaymentMethod_ShouldFail(int method)
    {
        var result = SalePayment.Create(1, method, 25m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("SalePayment.InvalidMethod");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Create_WithBoundaryValidPaymentMethod_ShouldSucceed(int method)
    {
        var result = SalePayment.Create(1, method, 25m);

        result.IsSuccess.Should().BeTrue();
        result.Value.PaymentMethod.Should().Be(method);
    }
}
