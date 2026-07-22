namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceCategory.Deactivate;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class DeactivateServiceCategoryCommandHandlerTests
{
    private static ServiceCategory CreateServiceCategory() =>
        ServiceCategory.Create(1, "Lavagem", "Servicos de lavagem").Value;

    [Fact]
    public async Task Handle_WithValidServiceCategory_ShouldDeactivate()
    {
        // Arrange
        var repository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var category = CreateServiceCategory();

        repository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new DeactivateServiceCategoryCommandHandler(repository, unitOfWork);

        var command = new DeactivateServiceCategoryCommand(category.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.IsActive.Should().BeFalse();

        await repository.Received(1).UpdateAsync(category, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithServiceCategoryNotFound_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((ServiceCategory?)null);

        var handler = new DeactivateServiceCategoryCommandHandler(repository, unitOfWork);

        var command = new DeactivateServiceCategoryCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
