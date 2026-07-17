namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class WashOperationalCostTests
{
    private static readonly DateTime MonthYear = new(2026, 7, 1);

    [Fact]
    public void Create_WithValidData_ShouldSucceedAndComputeTotalCost()
    {
        var result = WashOperationalCost.Create(1, MonthYear, 100m, 50m, 20m, 10m, 500m);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.MonthYear.Should().Be(MonthYear);
        result.Value.LaborCost.Should().Be(100m);
        result.Value.MaterialCost.Should().Be(50m);
        result.Value.EquipmentCost.Should().Be(20m);
        result.Value.UtilitiesCost.Should().Be(10m);
        result.Value.TotalCost.Should().Be(180m);
        result.Value.TotalRevenue.Should().Be(500m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidBranch_ShouldFail()
    {
        var result = WashOperationalCost.Create(0, MonthYear, 100m, 50m, 20m, 10m, 500m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashOperationalCost.InvalidBranch");
    }

    [Theory]
    [InlineData(-1, 50, 20, 10)]
    [InlineData(100, -1, 20, 10)]
    [InlineData(100, 50, -1, 10)]
    [InlineData(100, 50, 20, -1)]
    public void Create_WithNegativeCost_ShouldFail(decimal labor, decimal material, decimal equipment, decimal utilities)
    {
        var result = WashOperationalCost.Create(1, MonthYear, labor, material, equipment, utilities, 500m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashOperationalCost.InvalidCost");
    }

    [Fact]
    public void Create_WithNegativeRevenue_ShouldFail()
    {
        var result = WashOperationalCost.Create(1, MonthYear, 100m, 50m, 20m, 10m, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashOperationalCost.InvalidRevenue");
    }

    [Fact]
    public void NetProfit_ShouldBeRevenueMinusTotalCost()
    {
        var cost = WashOperationalCost.Create(1, MonthYear, 100m, 50m, 20m, 10m, 500m).Value;

        cost.NetProfit.Should().Be(320m);
    }

    [Fact]
    public void ProfitMargin_WithPositiveRevenue_ShouldComputePercentageRounded()
    {
        var cost = WashOperationalCost.Create(1, MonthYear, 100m, 50m, 20m, 10m, 500m).Value;

        cost.ProfitMargin.Should().Be(64.00m);
    }

    [Fact]
    public void ProfitMargin_WithZeroRevenue_ShouldBeZero()
    {
        var cost = WashOperationalCost.Create(1, MonthYear, 0m, 0m, 0m, 0m, 0m).Value;

        cost.ProfitMargin.Should().Be(0);
    }
}
