namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Parking.Domain.ValueObjects;
using Xunit;

public sealed class AppUserTests
{
    private static AppUser CreateUser(string passwordHash = "hashed-password") =>
        AppUser.Create(
            Username.Create("john.doe").Value,
            Email.Create("john.doe@example.com").Value,
            passwordHash,
            "John Doe").Value;

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var userName = Username.Create("john.doe").Value;
        var email = Email.Create("john.doe@example.com").Value;

        var result = AppUser.Create(userName, email, "hashed-password", "John Doe");

        result.IsSuccess.Should().BeTrue();
        result.Value.UserName.Should().Be(userName);
        result.Value.Email.Should().Be(email);
        result.Value.PasswordHash.Should().Be("hashed-password");
        result.Value.FullName.Should().Be("John Doe");
        result.Value.IsActive.Should().BeTrue();
        result.Value.FailedAccessCount.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_ShouldFail()
    {
        var userName = Username.Create("john.doe").Value;
        var email = Email.Create("john.doe@example.com").Value;

        var result = AppUser.Create(userName, email, "", "John Doe");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AppUser.InvalidPassword");
    }

    [Fact]
    public void Create_WithWhitespacePasswordHash_ShouldFail()
    {
        var userName = Username.Create("john.doe").Value;
        var email = Email.Create("john.doe@example.com").Value;

        var result = AppUser.Create(userName, email, "   ", "John Doe");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AppUser.InvalidPassword");
    }

    [Fact]
    public void IncrementFailedAccessCount_BelowThreshold_ShouldNotLockAccount()
    {
        var user = CreateUser();

        user.IncrementFailedAccessCount();

        user.FailedAccessCount.Should().Be(1);
        user.IsActive.Should().BeTrue();
        user.LockoutEndAt.Should().BeNull();
    }

    [Fact]
    public void IncrementFailedAccessCount_ReachingFive_ShouldLockAccount()
    {
        var user = CreateUser();

        for (var i = 0; i < 5; i++)
            user.IncrementFailedAccessCount();

        user.FailedAccessCount.Should().Be(5);
        user.IsActive.Should().BeFalse();
        user.LockoutEndAt.Should().NotBeNull();
    }

    [Fact]
    public void ResetFailedAccessCount_ShouldClearCountAndReactivate()
    {
        var user = CreateUser();
        for (var i = 0; i < 5; i++)
            user.IncrementFailedAccessCount();

        user.ResetFailedAccessCount();

        user.FailedAccessCount.Should().Be(0);
        user.LockoutEndAt.Should().BeNull();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void SetLastLogin_ShouldSetLastLoginAt()
    {
        var user = CreateUser();

        user.SetLastLogin();

        user.LastLoginAt.Should().NotBeNull();
    }

    [Fact]
    public void AssignRole_ShouldUpdateUpdatedAt()
    {
        var user = CreateUser();

        user.AssignRole(1);

        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalseAndUpdatedAt()
    {
        var user = CreateUser();

        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void IsLockedOut_WhenActiveEvenWithLockoutEnd_ShouldBeFalse()
    {
        var user = CreateUser();

        user.IsLockedOut.Should().BeFalse();
    }

    [Fact]
    public void IsLockedOut_WhenInactiveWithFutureLockoutEnd_ShouldBeTrue()
    {
        var user = CreateUser();
        for (var i = 0; i < 5; i++)
            user.IncrementFailedAccessCount();

        user.IsLockedOut.Should().BeTrue();
    }
}
