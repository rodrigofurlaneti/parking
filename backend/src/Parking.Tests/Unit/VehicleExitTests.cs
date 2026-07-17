namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class VehicleExitTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = VehicleExit.Create(1, 60, 10m, 1);

        result.IsSuccess.Should().BeTrue();
        result.Value.VehicleEntryId.Should().Be(1);
        result.Value.DurationMinutes.Should().Be(60);
        result.Value.TotalAmount.Should().Be(10m);
        result.Value.ParkingMode.Should().Be(1);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithZeroDuration_ShouldSucceed()
    {
        // Saida no mesmo minuto da entrada (ex.: cliente desiste e sai logo em seguida)
        // e um cenario valido e nao deve ser rejeitado.
        var result = VehicleExit.Create(1, 0, 10m, 1);

        result.IsSuccess.Should().BeTrue();
        result.Value.DurationMinutes.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeDuration_ShouldFail()
    {
        var result = VehicleExit.Create(1, -5, 10m, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleExit.InvalidDuration");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        var result = VehicleExit.Create(1, 60, -1m, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleExit.InvalidAmount");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldSucceed()
    {
        var result = VehicleExit.Create(1, 60, 0m, 1);

        result.IsSuccess.Should().BeTrue();
    }
}
