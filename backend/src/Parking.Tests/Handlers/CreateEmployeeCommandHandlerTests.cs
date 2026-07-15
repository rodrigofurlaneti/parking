namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.CreateEmployee;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class CreateEmployeeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        employeeRepository.GetByCPFAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Employee?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new CreateEmployeeCommandHandler(employeeRepository, unitOfWork);
        var command = new CreateEmployeeCommand(1, 1, "John Doe", "john@example.com", "123456789", "12345678901", 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("John Doe");
        result.Value.CPF.Should().Be("12345678901");
    }

    [Fact]
    public async Task Handle_WithDuplicateCPF_ShouldFail()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existingEmployee = Substitute.For<Domain.Entities.Employee>();
        employeeRepository.GetByCPFAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(existingEmployee);

        var handler = new CreateEmployeeCommandHandler(employeeRepository, unitOfWork);
        var command = new CreateEmployeeCommand(1, 1, "John Doe", "john@example.com", "123456789", "12345678901", 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.DuplicateCPF");
    }
}
