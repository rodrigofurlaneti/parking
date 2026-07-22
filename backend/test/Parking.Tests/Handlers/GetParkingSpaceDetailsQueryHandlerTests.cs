namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ParkingSpace.GetParkingSpaceDetails;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetParkingSpaceDetailsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithAvailableSpace_ShouldReturnDetailsWithAvailableStatus()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;

        parkingSpaceRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(parkingSpace);

        var handler = new GetParkingSpaceDetailsQueryHandler(parkingSpaceRepository);

        var query = new GetParkingSpaceDetailsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SpaceNumber.Should().Be("A1");
        result.Value.StatusDescription.Should().Be("Available");
    }

    [Fact]
    public async Task Handle_WithOccupiedSpace_ShouldReturnDetailsWithOccupiedStatus()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        parkingSpace.MarkAsOccupied();

        parkingSpaceRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(parkingSpace);

        var handler = new GetParkingSpaceDetailsQueryHandler(parkingSpaceRepository);

        var query = new GetParkingSpaceDetailsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StatusDescription.Should().Be("Occupied");
    }

    [Fact]
    public async Task Handle_WithMaintenanceSpace_ShouldReturnDetailsWithMaintenanceStatus()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var parkingSpace = ParkingSpace.Create(1, "A1", 1).Value;
        parkingSpace.MarkAsMaintenance();

        parkingSpaceRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(parkingSpace);

        var handler = new GetParkingSpaceDetailsQueryHandler(parkingSpaceRepository);

        var query = new GetParkingSpaceDetailsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StatusDescription.Should().Be("Maintenance");
    }

    [Fact]
    public async Task Handle_WithUnknownSpace_ShouldFail()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();

        parkingSpaceRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((ParkingSpace?)null);

        var handler = new GetParkingSpaceDetailsQueryHandler(parkingSpaceRepository);

        var query = new GetParkingSpaceDetailsQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.NotFound");
    }
}
