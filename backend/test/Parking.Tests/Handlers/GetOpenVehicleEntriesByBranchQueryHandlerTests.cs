namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.VehicleEntry.GetOpenByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetOpenVehicleEntriesByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithOpenEntriesInBranch_ShouldReturnDtos()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        var entry = VehicleEntry.Create(1, 1, 1, "ABC1D23", "Gol", "Prata").Value;

        vehicleEntryRepository.GetParkedByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<VehicleEntry> { entry });

        var handler = new GetOpenVehicleEntriesByBranchQueryHandler(vehicleEntryRepository);
        var query = new GetOpenVehicleEntriesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].LicensePlate.Should().Be("ABC1D23");
    }

    [Fact]
    public async Task Handle_WithNoOpenEntriesInBranch_ShouldReturnEmptyList()
    {
        // Arrange
        var vehicleEntryRepository = Substitute.For<IVehicleEntryRepository>();
        vehicleEntryRepository.GetParkedByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<VehicleEntry>());

        var handler = new GetOpenVehicleEntriesByBranchQueryHandler(vehicleEntryRepository);
        var query = new GetOpenVehicleEntriesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
