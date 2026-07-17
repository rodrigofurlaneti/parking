namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.GetById;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetEmployeeByIdQueryHandlerTests
{
    private static Employee CreateEmployee() =>
        Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;

    [Fact]
    public async Task Handle_WithExistingEmployee_ShouldReturnDto()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();

        var employee = CreateEmployee();

        employeeRepository.GetByIdAsync(employee.Id, Arg.Any<CancellationToken>()).Returns(employee);

        var handler = new GetEmployeeByIdQueryHandler(employeeRepository);

        // Act
        var result = await handler.Handle(new GetEmployeeByIdQuery(employee.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(employee.Id);
        result.Value.Name.Should().Be("John Doe");
        result.Value.CPF.Should().Be("12345678900");
    }

    [Fact]
    public async Task Handle_WithEmployeeNotFound_ShouldFail()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();

        employeeRepository.GetByIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns((Employee?)null);

        var handler = new GetEmployeeByIdQueryHandler(employeeRepository);

        // Act
        var result = await handler.Handle(new GetEmployeeByIdQuery(999), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.NotFound");
    }
}
