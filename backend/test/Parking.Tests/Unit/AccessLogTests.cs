namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class AccessLogTests
{
    [Fact]
    public void Constructor_WithRequiredData_ShouldAssignProperties()
    {
        var log = new AccessLog(1, "Login");

        log.UserId.Should().Be(1);
        log.Action.Should().Be("Login");
        log.Resource.Should().BeNull();
        log.IpAddress.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithOptionalData_ShouldAssignAllProperties()
    {
        var log = new AccessLog(1, "Login", "AppUser", "127.0.0.1");

        log.UserId.Should().Be(1);
        log.Action.Should().Be("Login");
        log.Resource.Should().Be("AppUser");
        log.IpAddress.Should().Be("127.0.0.1");
    }
}
