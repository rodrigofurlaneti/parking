namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class WashProductConsumptionTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = WashProductConsumption.Create(1, 2, 3m, 4m);

        result.IsSuccess.Should().BeTrue();
        result.Value.WashScheduleId.Should().Be(1);
        result.Value.ProductId.Should().Be(2);
        result.Value.QuantityUsed.Should().Be(3m);
        result.Value.UnitCost.Should().Be(4m);
        result.Value.TotalCost.Should().Be(12m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidSchedule_ShouldFail()
    {
        var result = WashProductConsumption.Create(0, 2, 3m, 4m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashProductConsumption.InvalidSchedule");
    }

    [Fact]
    public void Create_WithInvalidProduct_ShouldFail()
    {
        var result = WashProductConsumption.Create(1, 0, 3m, 4m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashProductConsumption.InvalidProduct");
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldFail()
    {
        var result = WashProductConsumption.Create(1, 2, 0m, 4m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashProductConsumption.InvalidQuantity");
    }

    [Fact]
    public void Create_WithNegativeUnitCost_ShouldFail()
    {
        var result = WashProductConsumption.Create(1, 2, 3m, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashProductConsumption.InvalidUnitCost");
    }
}
