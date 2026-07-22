namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class ServiceCategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = ServiceCategory.Create(1, "Wash", "Car wash services");

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.Name.Should().Be("Wash");
        result.Value.Description.Should().Be("Car wash services");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidBranch_ShouldFail()
    {
        var result = ServiceCategory.Create(0, "Wash", null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.InvalidBranch");
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = ServiceCategory.Create(1, "", null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.InvalidName");
    }

    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var category = ServiceCategory.Create(1, "Wash", null).Value;

        var result = category.Update("Deluxe Wash", "Premium wash");

        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("Deluxe Wash");
        category.Description.Should().Be("Premium wash");
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var category = ServiceCategory.Create(1, "Wash", null).Value;

        var result = category.Update("  ", null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.InvalidName");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var category = ServiceCategory.Create(1, "Wash", null).Value;

        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var category = ServiceCategory.Create(1, "Wash", null).Value;
        category.Deactivate();

        category.Activate();

        category.IsActive.Should().BeTrue();
    }
}
