namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class RoleTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var result = Role.Create("Manager", "Manages the branch");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Manager");
        result.Value.Description.Should().Be("Manages the branch");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithoutDescription_ShouldSucceedWithNullDescription()
    {
        var result = Role.Create("Manager");

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = Role.Create("");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Role.InvalidName");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldFail()
    {
        var result = Role.Create("   ");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Role.InvalidName");
    }
}
