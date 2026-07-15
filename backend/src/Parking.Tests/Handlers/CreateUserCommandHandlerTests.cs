namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Auth.CreateUser;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        passwordHasher.Hash(Arg.Any<string>())
            .Returns("hashed_password");
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateUserCommandHandler(userRepository, passwordHasher, unitOfWork);
        var command = new CreateUserCommand("testuser", "test@example.com", "Test User", "Password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await userRepository.Received(1).AddAsync(Arg.Any<dynamic>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepository.ExistsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var handler = new CreateUserCommandHandler(userRepository, passwordHasher, unitOfWork);
        var command = new CreateUserCommand("testuser", "test@example.com", "Test User", "Password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AppUser.DuplicateEmail");
    }
}
