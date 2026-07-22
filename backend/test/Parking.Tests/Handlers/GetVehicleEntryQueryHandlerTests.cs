namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleEntry.GetVehicleEntry;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetVehicleEntryQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingEntry_ShouldReturnDto()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var entry = VehicleEntry.Create(1, 1, 1, "ABC1D23", "Gol", "Prata").Value;

        vehicleEntryRepository.GetByIdAsync(entry.Id, Arg.Any<CancellationToken>()).Returns(entry);

        var handler = new GetVehicleEntryQueryHandler(vehicleEntryRepository);
        var query = new GetVehicleEntryQuery(entry.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC1D23");
        result.Value.VehicleModel.Should().Be("Gol");
    }

    [Fact]
    public async Task Handle_WithEntryNotFound_ShouldFail()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        vehicleEntryRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((VehicleEntry?)null);

        var handler = new GetVehicleEntryQueryHandler(vehicleEntryRepository);
        var query = new GetVehicleEntryQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleEntry.NotFound");
    }
}
