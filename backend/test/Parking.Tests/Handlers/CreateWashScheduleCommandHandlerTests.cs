namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashSchedule.Create;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateWashScheduleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateWashSchedule()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateWashScheduleCommandHandler(washScheduleRepository, unitOfWork);
        var command = new CreateWashScheduleCommand(1, 1, DateTime.UtcNow.AddHours(1), 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.VehicleEntryId.Should().Be(1);
        result.Value.EmployeeId.Should().Be(1);
        result.Value.Status.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithInvalidEmployee_ShouldFail()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateWashScheduleCommandHandler(washScheduleRepository, unitOfWork);
        var command = new CreateWashScheduleCommand(1, 1, DateTime.UtcNow.AddHours(1), 0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidEmployee");
    }
}
