namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleModel.Search;
using Parking.Domain.Repositories;
using Xunit;
using DomainVehicleModel = Parking.Domain.Entities.VehicleModel;

public sealed class SearchVehicleModelsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithEmptyQuery_ShouldReturnEmptyList_WithoutCallingRepository()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();

        var handler = new SearchVehicleModelsQueryHandler(vehicleModelRepository);

        // Act
        var result = await handler.Handle(new SearchVehicleModelsQuery(string.Empty), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        await vehicleModelRepository.DidNotReceive().SearchAsync(
            Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithWhitespaceQuery_ShouldReturnEmptyList()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();

        var handler = new SearchVehicleModelsQueryHandler(vehicleModelRepository);

        // Act
        var result = await handler.Handle(new SearchVehicleModelsQuery("   "), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithMatchingQuery_ShouldReturnFilteredResults()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();

        var model = DomainVehicleModel.Create("Gol").Value;

        vehicleModelRepository.SearchAsync("go", 10, Arg.Any<CancellationToken>())
            .Returns(new List<DomainVehicleModel> { model });

        var handler = new SearchVehicleModelsQueryHandler(vehicleModelRepository);

        // Act
        var result = await handler.Handle(new SearchVehicleModelsQuery("go"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].Name.Should().Be("Gol");
    }

    [Fact]
    public async Task Handle_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var vehicleModelRepository = Substitute.For<IVehicleModelRepository>();

        vehicleModelRepository.SearchAsync("zzz", 10, Arg.Any<CancellationToken>())
            .Returns(new List<DomainVehicleModel>());

        var handler = new SearchVehicleModelsQueryHandler(vehicleModelRepository);

        // Act
        var result = await handler.Handle(new SearchVehicleModelsQuery("zzz"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
