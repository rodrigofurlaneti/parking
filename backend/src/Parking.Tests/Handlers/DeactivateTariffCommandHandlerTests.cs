namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Tariff.DeactivateTariff;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class DeactivateTariffCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidId_ShouldDeactivateTariff()
    {
        // Arrange
        var repository = Substitute.For<ITariffRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var tariff = Domain.Entities.Tariff.Create(1, 5m, 2m, 30m).Value;

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(tariff);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new DeactivateTariffCommandHandler(repository, unitOfWork);
        var command = new DeactivateTariffCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        tariff.IsActive.Should().BeFalse();

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

        var handler = new DeactivateTariffCommandHandler(repository, unitOfWork);
        var command = new DeactivateTariffCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Tariff.NotFound");
    }
}
