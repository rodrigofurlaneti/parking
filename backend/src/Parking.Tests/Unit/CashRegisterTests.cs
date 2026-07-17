namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class CashRegisterTests
{
    [Fact]
    public void Create_WithValidOpeningBalance_ShouldSucceed()
    {
        var result = CashRegister.Create(1, 2, 100m);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.EmployeeId.Should().Be(2);
        result.Value.OpeningBalance.Should().Be(100m);
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithZeroOpeningBalance_ShouldSucceed()
    {
        var result = CashRegister.Create(1, 2, 0m);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativeOpeningBalance_ShouldFail()
    {
        var result = CashRegister.Create(1, 2, -10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("CashRegister.InvalidBalance");
    }

    [Fact]
    public void Close_ShouldSetClosingBalanceAndStatus()
    {
        var cashRegister = CashRegister.Create(1, 2, 100m).Value;

        cashRegister.Close(250m);

        cashRegister.ClosingBalance.Should().Be(250m);
        cashRegister.Status.Should().Be(1);
        cashRegister.ClosedAt.Should().NotBeNull();
    }
}
