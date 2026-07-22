namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllEmployeesByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithEmployeesInBranch_ShouldReturnDtos()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        var employee = Employee.Create(1, 1, "John Doe", "john@doe.com", "11999999999", "12345678900", 1).Value;

        employeeRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Employee> { employee });

        var handler = new GetAllEmployeesByBranchQueryHandler(employeeRepository);
        var query = new GetAllEmployeesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handle_WithNoEmployeesInBranch_ShouldReturnEmptyList()
    {
        // Arrange
        var employeeRepository = Substitute.For<IEmployeeRepository>();
        employeeRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<Employee>());

        var handler = new GetAllEmployeesByBranchQueryHandler(employeeRepository);
        var query = new GetAllEmployeesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
