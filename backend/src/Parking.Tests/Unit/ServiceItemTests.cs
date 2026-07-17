namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class ServiceItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = ServiceItem.Create(1, "Full Wash", "Complete wash", 30, 20m);

        result.IsSuccess.Should().BeTrue();
        result.Value.ServiceCategoryId.Should().Be(1);
        result.Value.Name.Should().Be("Full Wash");
        result.Value.Description.Should().Be("Complete wash");
        result.Value.DurationMinutes.Should().Be(30);
        result.Value.BaseCost.Should().Be(20m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidCategory_ShouldFail()
    {
        var result = ServiceItem.Create(0, "Full Wash", null, 30, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidCategory");
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = ServiceItem.Create(1, "", null, 30, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidName");
    }

    [Fact]
    public void Create_WithZeroDuration_ShouldFail()
    {
        var result = ServiceItem.Create(1, "Full Wash", null, 0, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidDuration");
    }

    [Fact]
    public void Create_WithNegativeCost_ShouldFail()
    {
        var result = ServiceItem.Create(1, "Full Wash", null, 30, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidCost");
    }

    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var item = ServiceItem.Create(1, "Full Wash", null, 30, 20m).Value;

        var result = item.Update("Deluxe Wash", "Better wash", 45, 30m);

        result.IsSuccess.Should().BeTrue();
        item.Name.Should().Be("Deluxe Wash");
        item.Description.Should().Be("Better wash");
        item.DurationMinutes.Should().Be(45);
        item.BaseCost.Should().Be(30m);
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var item = ServiceItem.Create(1, "Full Wash", null, 30, 20m).Value;

        var result = item.Update("", null, 45, 30m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidName");
    }

    [Fact]
    public void Update_WithZeroDuration_ShouldFail()
    {
        var item = ServiceItem.Create(1, "Full Wash", null, 30, 20m).Value;

        var result = item.Update("Full Wash", null, 0, 30m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidDuration");
    }

    [Fact]
    public void Update_WithNegativeCost_ShouldFail()
    {
        var item = ServiceItem.Create(1, "Full Wash", null, 30, 20m).Value;

        var result = item.Update("Full Wash", null, 30, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidCost");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var item = ServiceItem.Create(1, "Full Wash", null, 30, 20m).Value;

        item.Deactivate();

        item.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var item = ServiceItem.Create(1, "Full Wash", null, 30, 20m).Value;
        item.Deactivate();

        item.Activate();

        item.IsActive.Should().BeTrue();
    }
}
