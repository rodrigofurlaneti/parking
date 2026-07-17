namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Tariff.UpdateTariff;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateTariffCommandHandlerTests
{
    private static Tariff CreateTariff() =>
        Domain.Entities.Tariff.Create(1, 5m, 2m, 30m).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateTariff()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var tariff = CreateTariff();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(tariff);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new UpdateTariffCommandHandler(repository, unitOfWork);
        var command = new UpdateTariffCommand(1, 8m, 3m, 40m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FirstHourRate.Should().Be(8m);
        result.Value.AdditionalHourRate.Should().Be(3m);
        result.Value.DailyMaxRate.Should().Be(40m);

        await repository.Received(1).UpdateAsync(Arg.Any<Tariff>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentTariff_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns((Tariff?)null);

        var handler = new UpdateTariffCommandHandler(repository, unitOfWork);
        var command = new UpdateTariffCommand(1, 8m, 3m, 40m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.NotFound");
    }

    [Fact]
    public async Task Handle_WithNegativeFirstHourRate_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var tariff = CreateTariff();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(tariff);

        var handler = new UpdateTariffCommandHandler(repository, unitOfWork);
        var command = new UpdateTariffCommand(1, -5m, 3m, 40m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidFirstHourRate");
    }

    [Fact]
    public async Task Handle_WithNegativeAdditionalHourRate_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var tariff = CreateTariff();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(tariff);

        var handler = new UpdateTariffCommandHandler(repository, unitOfWork);
        var command = new UpdateTariffCommand(1, 8m, -3m, 40m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidAdditionalHourRate");
    }

    [Fact]
    public async Task Handle_WithInvalidDailyMaxRate_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var tariff = CreateTariff();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(tariff);

        var handler = new UpdateTariffCommandHandler(repository, unitOfWork);
        var command = new UpdateTariffCommand(1, 8m, 3m, 0m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.InvalidDailyMaxRate");
    }
}
