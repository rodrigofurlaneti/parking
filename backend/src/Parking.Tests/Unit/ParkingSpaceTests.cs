namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class ParkingSpaceTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = ParkingSpace.Create(1, "A1", 1);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.SpaceNumber.Should().Be("A1");
        result.Value.Type.Should().Be(1);
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptySpaceNumber_ShouldFail()
    {
        var result = ParkingSpace.Create(1, "", 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.InvalidNumber");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Create_WithInvalidType_ShouldFail(int type)
    {
        var result = ParkingSpace.Create(1, "A1", type);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.InvalidType");
    }

    [Fact]
    public void MarkAsOccupied_ShouldSetStatusToOne()
    {
        var space = ParkingSpace.Create(1, "A1", 1).Value;

        space.MarkAsOccupied();

        space.Status.Should().Be(1);
    }

    [Fact]
    public void MarkAsAvailable_ShouldSetStatusToZero()
    {
        var space = ParkingSpace.Create(1, "A1", 1).Value;
        space.MarkAsOccupied();

        space.MarkAsAvailable();

        space.Status.Should().Be(0);
    }

    [Fact]
    public void MarkAsMaintenance_ShouldSetStatusToTwo()
    {
        var space = ParkingSpace.Create(1, "A1", 1).Value;

        space.MarkAsMaintenance();

        space.Status.Should().Be(2);
    }
}
