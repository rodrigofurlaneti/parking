namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class CashMovementTests
{
    [Fact]
    public void Create_WithValidEntry_ShouldSucceed()
    {
        var result = CashMovement.Create(1, CashMovement.Entry, 50m, "Cash in");

        result.IsSuccess.Should().BeTrue();
        result.Value.CashRegisterId.Should().Be(1);
        result.Value.Type.Should().Be(CashMovement.Entry);
        result.Value.Amount.Should().Be(50m);
        result.Value.Description.Should().Be("Cash in");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void Create_WithInvalidType_ShouldFail(int type)
    {
        var result = CashMovement.Create(1, type, 50m, "desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidType");
    }

    [Fact]
    public void Create_EntryWithZeroAmount_ShouldFail()
    {
        var result = CashMovement.Create(1, CashMovement.Entry, 0m, "desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidAmount");
    }

    [Fact]
    public void Create_EntryWithNegativeAmount_ShouldFail()
    {
        var result = CashMovement.Create(1, CashMovement.Entry, -5m, "desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidAmount");
    }

    [Fact]
    public void Create_ExitWithZeroAmount_ShouldFail()
    {
        var result = CashMovement.Create(1, CashMovement.Exit, 0m, "desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidAmount");
    }

    [Fact]
    public void Create_AdjustmentWithZeroAmount_ShouldFail()
    {
        var result = CashMovement.Create(1, CashMovement.Adjustment, 0m, "desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashMovement.InvalidAmount");
    }

    [Fact]
    public void Create_AdjustmentWithNegativeAmount_ShouldSucceed()
    {
        var result = CashMovement.Create(1, CashMovement.Adjustment, -10m, "correction");

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(-10m);
    }

    [Fact]
    public void Create_AdjustmentWithPositiveAmount_ShouldSucceed()
    {
        var result = CashMovement.Create(1, CashMovement.Adjustment, 10m, "correction");

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(10m);
    }
}
