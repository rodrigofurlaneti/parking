namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.ServiceCategory.Create;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateServiceCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateServiceCategory()
    {
        // Arrange
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateServiceCategoryCommandHandler(serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceCategoryCommand(1, "Lavagem Rápida", "Lavagem externa rápida");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Lavagem Rápida");
        result.Value.BranchId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithInvalidBranch_ShouldFail()
    {
        // Arrange
        var serviceCategoryRepository = Substitute.For<IServiceCategoryRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateServiceCategoryCommandHandler(serviceCategoryRepository, unitOfWork);
        var command = new CreateServiceCategoryCommand(0, "Lavagem Rápida", "Lavagem externa rápida");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ServiceCategory.InvalidBranch");
    }
}
