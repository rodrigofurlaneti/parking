namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class RolePermissionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldAssignProperties()
    {
        var rolePermission = new RolePermission(1, 2);

        rolePermission.RoleId.Should().Be(1);
        rolePermission.PermissionId.Should().Be(2);
    }
}
