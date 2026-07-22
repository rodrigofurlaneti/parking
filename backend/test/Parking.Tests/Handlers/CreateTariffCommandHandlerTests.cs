namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Tariff.CreateTariff;
using Parking.Domain.Repositories;
using Xunit;
using DomainTariff = Parking.Domain.Entities.Tariff;

public sealed class CreateTariffCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidDataAndNoExistingActive_ShouldCreateTariff()
    {
        // Arrange
        var tariffRepository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        tariffRepository.GetActiveByBranchAsync(1, Arg.Any<CancellationToken>()).Returns((DomainTariff?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateTariffCommandHandler(tariffRepository, unitOfWork);

        var command = new CreateTariffCommand(1, 10m, 5m, 50m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.FirstHourRate.Should().Be(10m);
        result.Value.IsActive.Should().BeTrue();

        await tariffRepository.Received(1).AddAsync(Arg.Any<DomainTariff>(), Arg.Any<CancellationToken>());
        await tariffRepository.DidNotReceive().UpdateAsync(Arg.Any<DomainTariff>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingActiveTariff_ShouldDeactivateOldAndCreateNew()
    {
        // Arrange
        var tariffRepository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existingActive = DomainTariff.Create(1, 8m, 4m, null).Value;
        tariffRepository.GetActiveByBranchAsync(1, Arg.Any<CancellationToken>()).Returns(existingActive);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateTariffCommandHandler(tariffRepository, unitOfWork);

        var command = new CreateTariffCommand(1, 10m, 5m, 50m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingActive.IsActive.Should().BeFalse();

        await tariffRepository.Received(1).UpdateAsync(existingActive, Arg.Any<CancellationToken>());
        await tariffRepository.Received(1).AddAsync(Arg.Any<DomainTariff>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNegativeFirstHourRate_ShouldFail()
    {
        // Arrange
        var tariffRepository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateTariffCommandHandler(tariffRepository, unitOfWork);

        var command = new CreateTariffCommand(1, -1m, 5m, 50m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidFirstHourRate");
        await tariffRepository.DidNotReceive().GetActiveByBranchAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidDailyMaxRate_ShouldFail()
    {
        // Arrange
        var tariffRepository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateTariffCommandHandler(tariffRepository, unitOfWork);

        var command = new CreateTariffCommand(1, 10m, 5m, 0m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidDailyMaxRate");
    }
}
