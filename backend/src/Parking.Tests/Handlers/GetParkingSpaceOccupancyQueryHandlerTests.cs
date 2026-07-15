namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ParkingSpace.GetParkingSpaceOccupancy;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetParkingSpaceOccupancyQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithValidBranchId_ShouldReturnOccupancy()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();

        var spaces = new List<ParkingSpace>
        {
            Substitute.For<ParkingSpace>(),
            Substitute.For<ParkingSpace>(),
            Substitute.For<ParkingSpace>()
        };

        parkingSpaceRepository.GetAllByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(spaces);
        parkingSpaceRepository.GetOccupiedCountAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new GetParkingSpaceOccupancyQueryHandler(parkingSpaceRepository);
        var query = new GetParkingSpaceOccupancyQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.TotalSpaces.Should().Be(3);
        result.Value.OccupiedSpaces.Should().Be(1);
        result.Value.AvailableSpaces.Should().Be(2);
        result.Value.OccupancyRate.Should().BeApproximately(33.33m, 0.01m);
    }
}
