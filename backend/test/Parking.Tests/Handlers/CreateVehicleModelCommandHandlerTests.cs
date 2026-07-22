namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleModel.CreateVehicleModel;
using Parking.Domain.Repositories;
using Xunit;
using DomainVehicleModel = Parking.Domain.Entities.VehicleModel;

public sealed class CreateVehicleModelCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNewName_ShouldCreateVehicleModel()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        vehicleModelRepository.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((DomainVehicleModel?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateVehicleModelCommandHandler(vehicleModelRepository, unitOfWork);

        var command = new CreateVehicleModelCommand("Gol");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Gol");
        result.Value.IsActive.Should().BeTrue();

        await vehicleModelRepository.Received(1).AddAsync(
            Arg.Any<DomainVehicleModel>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingNameCaseInsensitive_ShouldReturnExistingModel_AndNotDuplicate()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existing = DomainVehicleModel.Create("Gol").Value;

        vehicleModelRepository.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(existing);

        var handler = new CreateVehicleModelCommandHandler(vehicleModelRepository, unitOfWork);

        var command = new CreateVehicleModelCommand("gol");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(existing.Id);
        result.Value.Name.Should().Be(existing.Name);

        await vehicleModelRepository.DidNotReceive().AddAsync(
            Arg.Any<DomainVehicleModel>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        vehicleModelRepository.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((DomainVehicleModel?)null);

        var handler = new CreateVehicleModelCommandHandler(vehicleModelRepository, unitOfWork);

        var command = new CreateVehicleModelCommand(string.Empty);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleModel.InvalidName");

        await vehicleModelRepository.DidNotReceive().AddAsync(
            Arg.Any<DomainVehicleModel>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
