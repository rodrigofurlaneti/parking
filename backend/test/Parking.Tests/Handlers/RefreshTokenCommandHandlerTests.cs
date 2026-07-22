namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Auth.RefreshToken;
using Xunit;

public sealed class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithAnyRefreshToken_ShouldFail()
    {
        // Arrange
        var tokenService = Substitute.For<ITokenService>();
        var handler = new RefreshTokenCommandHandler(tokenService);

        var command = new RefreshTokenCommand("some-refresh-token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.InvalidRefreshToken");
    }

    [Fact]
    public async Task Handle_WithEmptyRefreshToken_ShouldFail()
    {
        // Arrange
        var tokenService = Substitute.For<ITokenService>();
        var handler = new RefreshTokenCommandHandler(tokenService);

        var command = new RefreshTokenCommand(string.Empty);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.InvalidRefreshToken");
    }
}
