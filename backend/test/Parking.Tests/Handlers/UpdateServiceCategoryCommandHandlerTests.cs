namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceCategory.Update;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateServiceCategoryCommandHandlerTests
{
    private static ServiceCategory CreateServiceCategory() =>
        ServiceCategory.Create(1, "Lavagem", "Servicos de lavagem").Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateServiceCategory()
    {
        // Arrange
        var repository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var category = CreateServiceCategory();

        repository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new UpdateServiceCategoryCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceCategoryCommand(category.Id, "Estetica", "Servicos de estetica");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Estetica");
        result.Value.Description.Should().Be("Servicos de estetica");

        await repository.Received(1).UpdateAsync(Arg.Any<ServiceCategory>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithServiceCategoryNotFound_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((ServiceCategory?)null);

        var handler = new UpdateServiceCategoryCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceCategoryCommand(999, "Estetica", "Servicos de estetica");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var category = CreateServiceCategory();

        repository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var handler = new UpdateServiceCategoryCommandHandler(repository, unitOfWork);

        var command = new UpdateServiceCategoryCommand(category.Id, string.Empty, "Servicos de estetica");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.InvalidName");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
