namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class UserRoleTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldAssignPropertiesAndAssignedAt()
    {
        var userRole = new UserRole(1, 2);

        userRole.UserId.Should().Be(1);
        userRole.RoleId.Should().Be(2);
        userRole.AssignedAt.Should().NotBe(default);
    }
}
