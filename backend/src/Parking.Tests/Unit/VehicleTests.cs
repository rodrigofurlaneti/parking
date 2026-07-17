namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class VehicleTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Vehicle.Create(1, "abc1234", "Gol", "Blue");

        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(1);
        result.Value.LicensePlate.Should().Be("ABC1234");
        result.Value.Model.Should().Be("Gol");
        result.Value.Color.Should().Be("Blue");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldTrimAndUppercasePlate()
    {
        var result = Vehicle.Create(1, "  abc1234  ", null, null);

        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC1234");
    }

    [Fact]
    public void Create_WithMissingPlate_ShouldFail()
    {
        var result = Vehicle.Create(1, "  ", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Vehicle.InvalidPlate");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var vehicle = Vehicle.Create(1, "ABC1234", null, null).Value;

        vehicle.Deactivate();

        vehicle.IsActive.Should().BeFalse();
    }
}
