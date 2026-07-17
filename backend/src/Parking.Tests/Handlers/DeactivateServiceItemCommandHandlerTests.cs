namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceItem.Deactivate;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class DeactivateServiceItemCommandHandlerTests
{
    private static ServiceItem CreateServiceItem() =>
        ServiceItem.Create(1, "Lavagem Simples", "Lavagem externa", 30, 25m).Value;

    [Fact]
    public async Task Handle_WithValidServiceItem_ShouldDeactivate()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var item = CreateServiceItem();

        repository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new DeactivateServiceItemCommandHandler(repository, unitOfWork);

        var command = new DeactivateServiceItemCommand(item.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.IsActive.Should().BeFalse();

        await repository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithServiceItemNotFound_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceItemRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((ServiceItem?)null);

        var handler = new DeactivateServiceItemCommandHandler(repository, unitOfWork);

        var command = new DeactivateServiceItemCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
