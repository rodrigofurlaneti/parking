namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.TerminateEmployee;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class TerminateEmployeeCommandHandlerTests
{
    private static Employee CreateEmployee() =>
        Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;

    [Fact]
    public async Task Handle_WithValidEmployee_ShouldTerminate()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var employee = CreateEmployee();

        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new TerminateEmployeeCommandHandler(employeeRepository, unitOfWork);

        var command = new TerminateEmployeeCommand(employee.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        employee.IsActive.Should().BeFalse();
        employee.TerminationDate.Should().NotBeNull();

        await employeeRepository.Received(1).UpdateAsync(employee, Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmployeeNotFound_ShouldFail()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        employeeRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Employee?)null);

        var handler = new TerminateEmployeeCommandHandler(employeeRepository, unitOfWork);

        var command = new TerminateEmployeeCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
