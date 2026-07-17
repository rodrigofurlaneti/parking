namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceItem.Update;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateServiceItemCommandHandlerTests
{
    private static ServiceItem CreateServiceItem() =>
        ServiceItem.Create(1, "Lavagem Simples", "Lavagem externa", 30, 25m).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateServiceItem()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var item = CreateServiceItem();

        repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new UpdateServiceItemCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceItemCommand(item.Id, "Lavagem Completa", "Lavagem completa", 60, 45m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Lavagem Completa");
        result.Value.DurationMinutes.Should().Be(60);
        result.Value.BaseCost.Should().Be(45m);

        await repository.Received(1).UpdateAsync(Arg.Any<ServiceItem>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithServiceItemNotFound_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((ServiceItem?)null);

        var handler = new UpdateServiceItemCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceItemCommand(999, "Lavagem Completa", "Lavagem completa", 60, 45m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var item = CreateServiceItem();

        repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var handler = new UpdateServiceItemCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceItemCommand(item.Id, string.Empty, "Lavagem completa", 60, 45m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidName");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidDuration_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var item = CreateServiceItem();

        repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var handler = new UpdateServiceItemCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceItemCommand(item.Id, "Lavagem Completa", "Lavagem completa", 0, 45m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidDuration");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNegativeBaseCost_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var item = CreateServiceItem();

        repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var handler = new UpdateServiceItemCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceItemCommand(item.Id, "Lavagem Completa", "Lavagem completa", 60, -10m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidCost");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
