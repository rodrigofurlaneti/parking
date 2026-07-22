namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Tariff.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllTariffsByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithTariffsInBranch_ShouldReturnList()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();

        var tariff1 = Domain.Entities.Tariff.Create(1, 5m, 2m, 30m).Value;
        var tariff2 = Domain.Entities.Tariff.Create(1, 6m, 2.5m, 35m).Value;

        repository.GetAllByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<Tariff> { tariff1, tariff2 });

        var handler = new GetAllTariffsByBranchQueryHandler(repository);
        var query = new GetAllTariffsByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoTariffs_ShouldReturnEmptyList()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();

        repository.GetAllByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(new List<Tariff>());

        var handler = new GetAllTariffsByBranchQueryHandler(repository);
        var query = new GetAllTariffsByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
