namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.UpdateEmployee;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class UpdateEmployeeCommandHandlerTests
{
    private static Employee CreateEmployee() =>
        Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateEmployee()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var employee = CreateEmployee();

        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new UpdateEmployeeCommandHandler(employeeRepository, unitOfWork);

        var command = new UpdateEmployeeCommand(employee.Id, "Jane Doe", "jane@doe.com", "11988887777", 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Jane Doe");
        result.Value.Email.Should().Be("jane@doe.com");
        result.Value.Phone.Should().Be("11988887777");
        result.Value.RoleId.Should().Be(2);

        await employeeRepository.Received(1).UpdateAsync(Arg.Any<Employee>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmployeeNotFound_ShouldFail()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        employeeRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Employee?)null);

        var handler = new UpdateEmployeeCommandHandler(employeeRepository, unitOfWork);

        var command = new UpdateEmployeeCommand(999, "Jane Doe", "jane@doe.com", "11988887777", 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.NotFound");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var employee = CreateEmployee();

        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new UpdateEmployeeCommandHandler(employeeRepository, unitOfWork);

        var command = new UpdateEmployeeCommand(employee.Id, string.Empty, "jane@doe.com", "11988887777", 2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.InvalidName");

        await unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
