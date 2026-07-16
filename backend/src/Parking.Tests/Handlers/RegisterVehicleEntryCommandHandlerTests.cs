namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class RegisterVehicleEntryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldRegisterVehicleEntry()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new RegisterVehicleEntryCommandHandler(vehicleEntryRepository, parkingSpaceRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LicensePlate.Should().Be("ABC-1234");
        result.Value.Status.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithUnavailableParkingSpace_ShouldFail()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var parkingSpace = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        parkingSpace.MarkAsOccupied(); // Occupied

        parkingSpaceRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(parkingSpace);

        var handler = new RegisterVehicleEntryCommandHandler(vehicleEntryRepository, parkingSpaceRepository, unitOfWork);
        var command = new RegisterVehicleEntryCommand(1, 1, 1, "ABC-1234", "Honda Civic", "Black");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.NotAvailable");
    }
}
