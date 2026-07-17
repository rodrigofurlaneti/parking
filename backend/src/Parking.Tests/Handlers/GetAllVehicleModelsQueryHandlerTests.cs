namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleModel.GetAll;
using Parking.Domain.Repositories;
using Xunit;
using DomainVehicleModel = Parking.Domain.Entities.VehicleModel;

public sealed class GetAllVehicleModelsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoModels_ShouldReturnEmptyList()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();

        vehicleModelRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<DomainVehicleModel>());

        var handler = new GetAllVehicleModelsQueryHandler(vehicleModelRepository);

        // Act
        var result = await handler.Handle(new GetAllVehicleModelsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithModels_ShouldReturnMappedList()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();

        var model1 = DomainVehicleModel.Create("Gol").Value;
        var model2 = DomainVehicleModel.Create("Onix").Value;

        vehicleModelRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<DomainVehicleModel> { model1, model2 });

        var handler = new GetAllVehicleModelsQueryHandler(vehicleModelRepository);

        // Act
        var result = await handler.Handle(new GetAllVehicleModelsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(x => x.Name).Should().Contain(new[] { "Gol", "Onix" });
    }
}
