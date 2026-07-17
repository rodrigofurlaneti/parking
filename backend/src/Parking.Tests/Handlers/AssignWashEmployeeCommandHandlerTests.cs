namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashSchedule.AssignEmployee;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class AssignWashEmployeeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldAssignEmployee()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var schedule = WashSchedule.Create(1, 1, DateTime.UtcNow, 1).Value;
        washScheduleRepository.GetByIdAsync(schedule.Id, Arg.Any<CancellationToken>()).Returns(schedule);

        var handler = new AssignWashEmployeeCommandHandler(washScheduleRepository, unitOfWork);
        var command = new AssignWashEmployeeCommand(schedule.Id, 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EmployeeId.Should().Be(2);
        await washScheduleRepository.Received(1).UpdateAsync(schedule, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithScheduleNotFound_ShouldFail()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        washScheduleRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((WashSchedule?)null);

        var handler = new AssignWashEmployeeCommandHandler(washScheduleRepository, unitOfWork);
        var command = new AssignWashEmployeeCommand(1, 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.NotFound");
    }

    [Fact]
    public async Task Handle_WithInvalidEmployeeId_ShouldFail()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var schedule = WashSchedule.Create(1, 1, DateTime.UtcNow, 1).Value;
        washScheduleRepository.GetByIdAsync(schedule.Id, Arg.Any<CancellationToken>()).Returns(schedule);

        var handler = new AssignWashEmployeeCommandHandler(washScheduleRepository, unitOfWork);
        var command = new AssignWashEmployeeCommand(schedule.Id, 0);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidEmployee");
    }

    [Fact]
    public async Task Handle_WithFinishedSchedule_ShouldFail()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var schedule = WashSchedule.Create(1, 1, DateTime.UtcNow, 1).Value;
        schedule.Start();
        schedule.Complete();
        washScheduleRepository.GetByIdAsync(schedule.Id, Arg.Any<CancellationToken>()).Returns(schedule);

        var handler = new AssignWashEmployeeCommandHandler(washScheduleRepository, unitOfWork);
        var command = new AssignWashEmployeeCommand(schedule.Id, 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }
}
