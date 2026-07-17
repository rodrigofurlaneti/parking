namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class PermissionTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var result = Permission.Create("vehicles.read", "Can read vehicles");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("vehicles.read");
        result.Value.Description.Should().Be("Can read vehicles");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithoutDescription_ShouldSucceedWithNullDescription()
    {
        var result = Permission.Create("vehicles.read");

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = Permission.Create("");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Permission.InvalidName");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldFail()
    {
        var result = Permission.Create("   ");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Permission.InvalidName");
    }
}
