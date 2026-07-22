namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Auth.GetUsers;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllUsers()
    {
        // Arrange
        var userRepository = Substitute.For<IAppUserRepository>();
        userRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<AppUser>());

        var handler = new GetAllUsersQueryHandler(userRepository);
        var query = new GetAllUsersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
