namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceItem.Create;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateServiceItemCommandHandlerTests
{
    private static Domain.Entities.ServiceCategory CreateCategory() =>
        Domain.Entities.ServiceCategory.Create(1, "Lavagem", "Servicos de lavagem").Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateServiceItem()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var category = CreateCategory();
        serviceCategoryRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(category);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateServiceItemCommandHandler(serviceItemRepository, serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceItemCommand(1, "Lavagem Simples", "Descricao", 30, 25m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Lavagem Simples");
        result.Value.DurationMinutes.Should().Be(30);
        result.Value.BaseCost.Should().Be(25m);
        result.Value.IsActive.Should().BeTrue();

        await serviceItemRepository.Received(1).AddAsync(
            Arg.Any<Domain.Entities.ServiceItem>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldFail()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        serviceCategoryRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Domain.Entities.ServiceCategory?)null);

        var handler = new CreateServiceItemCommandHandler(serviceItemRepository, serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceItemCommand(1, "Lavagem Simples", null, 30, 25m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.NotFound");
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        serviceCategoryRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateCategory());

        var handler = new CreateServiceItemCommandHandler(serviceItemRepository, serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceItemCommand(1, string.Empty, null, 30, 25m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidName");
    }

    [Fact]
    public async Task Handle_WithInvalidDuration_ShouldFail()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        serviceCategoryRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateCategory());

        var handler = new CreateServiceItemCommandHandler(serviceItemRepository, serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceItemCommand(1, "Lavagem Simples", null, 0, 25m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidDuration");
    }

    [Fact]
    public async Task Handle_WithNegativeCost_ShouldFail()
    {
        // Arrange
        var serviceItemRepository = Substitute.For<IServiceItemRepository>();
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        serviceCategoryRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(CreateCategory());

        var handler = new CreateServiceItemCommandHandler(serviceItemRepository, serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceItemCommand(1, "Lavagem Simples", null, 30, -1m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceItem.InvalidCost");
    }
}
