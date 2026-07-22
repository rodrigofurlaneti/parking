namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class VehicleEntryTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = VehicleEntry.Create(1, 2, 3, "ABC1234", "Gol", "Black");

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.ParkingSpaceId.Should().Be(2);
        result.Value.CustomerId.Should().Be(3);
        result.Value.LicensePlate.Should().Be("ABC1234");
        result.Value.VehicleModel.Should().Be("Gol");
        result.Value.VehicleColor.Should().Be("Black");
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyLicensePlate_ShouldFail()
    {
        var result = VehicleEntry.Create(1, 2, 3, "", "Gol", "Black");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleEntry.InvalidPlate");
    }

    [Fact]
    public void MarkAsExited_ShouldSetExitTimeAndStatus()
    {
        var entry = VehicleEntry.Create(1, 2, 3, "ABC1234", "Gol", "Black").Value;

        entry.MarkAsExited();

        entry.ExitTime.Should().NotBeNull();
        entry.Status.Should().Be(1);
    }

    [Fact]
    public void GetDurationMinutes_WithoutExit_ShouldComputeFromNow()
    {
        var entry = VehicleEntry.Create(1, 2, 3, "ABC1234", "Gol", "Black").Value;

        var duration = entry.GetDurationMinutes();

        duration.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void GetDurationMinutes_AfterExit_ShouldBeGreaterThanOrEqualToZero()
    {
        var entry = VehicleEntry.Create(1, 2, 3, "ABC1234", "Gol", "Black").Value;
        entry.MarkAsExited();

        var duration = entry.GetDurationMinutes();

        duration.Should().BeGreaterThanOrEqualTo(0);
    }
}
