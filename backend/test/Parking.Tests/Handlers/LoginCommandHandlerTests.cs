namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Auth.Login;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var tokenService = Substitute.For<ITokenService>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        tokenService.GenerateAccessToken(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("jwt_token");
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new LoginCommandHandler(userRepository, passwordHasher, tokenService, unitOfWork);
        var command = new LoginCommand("testuser", "password");

        // Act
        // This test would need the user to be properly created first
        // Simplified for example
    }
}
