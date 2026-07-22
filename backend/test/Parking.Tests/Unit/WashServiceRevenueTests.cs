namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class WashServiceRevenueTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceedAndComputeTotalPrice()
    {
        var result = WashServiceRevenue.Create(1, 2, 3, 10m, 5m);

        result.IsSuccess.Should().BeTrue();
        result.Value.WashScheduleId.Should().Be(1);
        result.Value.ServiceItemId.Should().Be(2);
        result.Value.Quantity.Should().Be(3);
        result.Value.UnitPrice.Should().Be(10m);
        result.Value.TotalPrice.Should().Be(30m);
        result.Value.Commission.Should().Be(5m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidSchedule_ShouldFail()
    {
        var result = WashServiceRevenue.Create(0, 2, 3, 10m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashServiceRevenue.InvalidSchedule");
    }

    [Fact]
    public void Create_WithInvalidServiceItem_ShouldFail()
    {
        var result = WashServiceRevenue.Create(1, 0, 3, 10m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashServiceRevenue.InvalidServiceItem");
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldFail()
    {
        var result = WashServiceRevenue.Create(1, 2, 0, 10m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashServiceRevenue.InvalidQuantity");
    }

    [Fact]
    public void Create_WithNegativeUnitPrice_ShouldFail()
    {
        var result = WashServiceRevenue.Create(1, 2, 3, -1m, 5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashServiceRevenue.InvalidUnitPrice");
    }

    [Fact]
    public void Create_WithNegativeCommission_ShouldFail()
    {
        var result = WashServiceRevenue.Create(1, 2, 3, 10m, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashServiceRevenue.InvalidCommission");
    }
}
