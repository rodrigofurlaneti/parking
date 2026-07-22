namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Branch.Update;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateBranchCommandHandlerTests
{
    private static Branch CreateBranch() =>
        Domain.Entities.Branch.Create(1, "Original Branch", "Street 1", 50).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateBranch()
    {
        // Arrange
        var repository = Substitute.For<IBranchRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var branch = CreateBranch();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(branch);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new UpdateBranchCommandHandler(repository, unitOfWork);
        var command = new UpdateBranchCommand(1, "Updated Branch", "Street 2", "11988887777", 100);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated Branch");
        result.Value.TotalSpaces.Should().Be(100);

        await repository.Received(1).UpdateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentBranch_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IBranchRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns((Branch?)null);

        var handler = new UpdateBranchCommandHandler(repository, unitOfWork);
        var command = new UpdateBranchCommand(1, "Updated Branch", "Street 2", "11988887777", 100);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.NotFound");
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IBranchRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var branch = CreateBranch();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        var handler = new UpdateBranchCommandHandler(repository, unitOfWork);
        var command = new UpdateBranchCommand(1, string.Empty, "Street 2", "11988887777", 100);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidName");
    }

    [Fact]
    public async Task Handle_WithInvalidTotalSpaces_ShouldFail()
    {
        // Arrange
        var repository = Substitute.For<IBranchRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var branch = CreateBranch();

        repository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(branch);

        var handler = new UpdateBranchCommandHandler(repository, unitOfWork);
        var command = new UpdateBranchCommand(1, "Updated Branch", "Street 2", "11988887777", 0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidTotalSpaces");
    }
}
