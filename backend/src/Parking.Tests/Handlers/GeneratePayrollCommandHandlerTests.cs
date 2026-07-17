namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.GeneratePayroll;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GeneratePayrollCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreatePayroll()
    {
        // Arrange
        var payrollRepository = Substitute.For<IEmployeePayrollRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var employee = Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;
        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new GeneratePayrollCommandHandler(payrollRepository, employeeRepository, unitOfWork);
        var command = new GeneratePayrollCommand(employee.Id, new DateTime(2026, 7, 1), 3000m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await payrollRepository.Received(1).AddAsync(Arg.Any<EmployeePayroll>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmployeeNotFound_ShouldFail()
    {
        // Arrange
        var payrollRepository = Substitute.For<IEmployeePayrollRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        employeeRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Employee?)null);

        var handler = new GeneratePayrollCommandHandler(payrollRepository, employeeRepository, unitOfWork);
        var command = new GeneratePayrollCommand(1, new DateTime(2026, 7, 1), 3000m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.NotFound");
    }

    [Fact]
    public async Task Handle_WithNonPositiveSalary_ShouldFail()
    {
        // Arrange
        var payrollRepository = Substitute.For<IEmployeePayrollRepository>();
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var employee = Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;
        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new GeneratePayrollCommandHandler(payrollRepository, employeeRepository, unitOfWork);
        var command = new GeneratePayrollCommand(employee.Id, new DateTime(2026, 7, 1), 0m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeePayroll.InvalidSalary");
    }
}
