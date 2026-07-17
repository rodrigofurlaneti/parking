namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ParkingSpace.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllParkingSpacesByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithSpacesInBranch_ShouldReturnDtos()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var space = ParkingSpace.Create(1, "A1", 1).Value;

        parkingSpaceRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<ParkingSpace> { space });

        var handler = new GetAllParkingSpacesByBranchQueryHandler(parkingSpaceRepository);
        var query = new GetAllParkingSpacesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].SpaceNumber.Should().Be("A1");
    }

    [Fact]
    public async Task Handle_WithNoSpacesInBranch_ShouldReturnEmptyList()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        parkingSpaceRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<ParkingSpace>());

        var handler = new GetAllParkingSpacesByBranchQueryHandler(parkingSpaceRepository);
        var query = new GetAllParkingSpacesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
