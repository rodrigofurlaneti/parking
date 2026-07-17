namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class VehicleModelTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var result = VehicleModel.Create("  Gol  ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Gol");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = VehicleModel.Create("  ");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleModel.InvalidName");
    }

    [Fact]
    public void Update_WithValidName_ShouldSucceed()
    {
        var model = VehicleModel.Create("Gol").Value;

        var result = model.Update("  Onix  ");

        result.IsSuccess.Should().BeTrue();
        model.Name.Should().Be("Onix");
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var model = VehicleModel.Create("Gol").Value;

        var result = model.Update(" ");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleModel.InvalidName");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var model = VehicleModel.Create("Gol").Value;

        model.Deactivate();

        model.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var model = VehicleModel.Create("Gol").Value;
        model.Deactivate();

        model.Activate();

        model.IsActive.Should().BeTrue();
    }
}
