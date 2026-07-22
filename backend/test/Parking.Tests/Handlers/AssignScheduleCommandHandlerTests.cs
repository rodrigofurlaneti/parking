namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.AssignSchedule;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class AssignScheduleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateSchedule()
    {
        // Arrange
        var scheduleRepository = Substitute.For<IEmployeeScheduleRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var employee = Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;
        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new AssignScheduleCommandHandler(scheduleRepository, employeeRepository, unitOfWork);
        var command = new AssignScheduleCommand(employee.Id, 1, new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await scheduleRepository.Received(1).AddAsync(Arg.Any<EmployeeSchedule>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmployeeNotFound_ShouldFail()
    {
        // Arrange
        var scheduleRepository = Substitute.For<IEmployeeScheduleRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        employeeRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Employee?)null);

        var handler = new AssignScheduleCommandHandler(scheduleRepository, employeeRepository, unitOfWork);
        var command = new AssignScheduleCommand(1, 1, new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.NotFound");
    }

    [Fact]
    public async Task Handle_WithInvalidDayOfWeek_ShouldFail()
    {
        // Arrange
        var scheduleRepository = Substitute.For<IEmployeeScheduleRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var employee = Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;
        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new AssignScheduleCommandHandler(scheduleRepository, employeeRepository, unitOfWork);
        var command = new AssignScheduleCommand(employee.Id, 7, new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeeSchedule.InvalidDay");
    }

    [Fact]
    public async Task Handle_WithStartTimeAfterEndTime_ShouldFail()
    {
        // Arrange
        var scheduleRepository = Substitute.For<IEmployeeScheduleRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var employee = Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;
        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new AssignScheduleCommandHandler(scheduleRepository, employeeRepository, unitOfWork);
        var command = new AssignScheduleCommand(employee.Id, 1, new TimeSpan(17, 0, 0), new TimeSpan(8, 0, 0));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeeSchedule.InvalidTime");
    }
}
