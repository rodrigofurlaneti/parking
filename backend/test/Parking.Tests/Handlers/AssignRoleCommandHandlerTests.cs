namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Auth.AssignRole;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Parking.Domain.ValueObjects;
using Xunit;

public sealed class AssignRoleCommandHandlerTests
{
    private static AppUser CreateUser() =>
        AppUser.Create(
            Username.Create("jdoe").Value,
            Email.Create("jdoe@parking.com").Value,
            "hashed-password",
            "John Doe").Value;

    [Fact]
    public async Task Handle_WithValidUserAndRole_ShouldAssignRole()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var roleRepository = Substitute.For<IRoleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var user = CreateUser();
        var role = Role.Create("Manager").Value;

        userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        roleRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(role);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new AssignRoleCommandHandler(userRepository, roleRepository, unitOfWork);

        var command = new AssignRoleCommand(1, 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        await userRepository.Received(1).AddRoleToUserAsync(1, 2, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownUser_ShouldFail()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var roleRepository = Substitute.For<IRoleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((AppUser?)null);

        var handler = new AssignRoleCommandHandler(userRepository, roleRepository, unitOfWork);

        var command = new AssignRoleCommand(1, 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AppUser.NotFound");
        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnknownRole_ShouldFail()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        var roleRepository = Substitute.For<IRoleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var user = CreateUser();

        userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        roleRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns((Role?)null);

        var handler = new AssignRoleCommandHandler(userRepository, roleRepository, unitOfWork);

        var command = new AssignRoleCommand(1, 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Role.NotFound");
        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
