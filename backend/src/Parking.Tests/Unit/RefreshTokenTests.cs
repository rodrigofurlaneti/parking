namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class RefreshTokenTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldAssignProperties()
    {
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var token = new RefreshToken(1, "some-token", expiresAt);

        token.UserId.Should().Be(1);
        token.Token.Should().Be("some-token");
        token.ExpiresAt.Should().Be(expiresAt);
        token.RevokedAt.Should().BeNull();
    }

    [Fact]
    public void IsExpired_WhenExpiresAtInFuture_ShouldBeFalse()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(1));

        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenExpiresAtInPast_ShouldBeTrue()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(-1));

        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsRevoked_WhenNotRevoked_ShouldBeFalse()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(1));

        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void Revoke_ShouldSetRevokedAtAndIsRevokedTrue()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(1));

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
        token.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public void IsValid_WhenNotExpiredAndNotRevoked_ShouldBeTrue()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(1));

        token.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhenRevoked_ShouldBeFalse()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        token.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhenExpired_ShouldBeFalse()
    {
        var token = new RefreshToken(1, "some-token", DateTime.UtcNow.AddDays(-1));

        token.IsValid.Should().BeFalse();
    }
}
