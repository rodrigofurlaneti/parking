namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ParkingSpace.CreateParkingSpace;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateParkingSpaceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateParkingSpace()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        parkingSpaceRepository.GetByNumberAsync(1, "A1", Arg.Any<CancellationToken>()).Returns((Domain.Entities.ParkingSpace?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateParkingSpaceCommandHandler(parkingSpaceRepository, unitOfWork);

        var command = new CreateParkingSpaceCommand(1, "A1", 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.SpaceNumber.Should().Be("A1");
        result.Value.Type.Should().Be(1);
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();

        await parkingSpaceRepository.Received(1).AddAsync(Arg.Any<Domain.Entities.ParkingSpace>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateNumber_ShouldFail()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existing = Domain.Entities.ParkingSpace.Create(1, "A1", 1).Value;
        parkingSpaceRepository.GetByNumberAsync(1, "A1", Arg.Any<CancellationToken>()).Returns(existing);

        var handler = new CreateParkingSpaceCommandHandler(parkingSpaceRepository, unitOfWork);

        var command = new CreateParkingSpaceCommand(1, "A1", 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.DuplicateNumber");
        await parkingSpaceRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.ParkingSpace>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidType_ShouldFail()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        parkingSpaceRepository.GetByNumberAsync(1, "A1", Arg.Any<CancellationToken>()).Returns((Domain.Entities.ParkingSpace?)null);

        var handler = new CreateParkingSpaceCommandHandler(parkingSpaceRepository, unitOfWork);

        var command = new CreateParkingSpaceCommand(1, "A1", 9);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.InvalidType");
    }

    [Fact]
    public async Task Handle_WithEmptySpaceNumber_ShouldFail()
    {
        // Arrange
        var parkingSpaceRepository = Substitute.For<IParkingSpaceRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        parkingSpaceRepository.GetByNumberAsync(1, string.Empty, Arg.Any<CancellationToken>()).Returns((Domain.Entities.ParkingSpace?)null);

        var handler = new CreateParkingSpaceCommandHandler(parkingSpaceRepository, unitOfWork);

        var command = new CreateParkingSpaceCommand(1, string.Empty, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ParkingSpace.InvalidNumber");
    }
}
