namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.GetPayroll;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetPayrollQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithPayrollsForEmployee_ShouldReturnDtos()
    {
        // Arrange
        var payrollRepository = Substitute.For<IEmployeePayrollRepository>();
        var payroll = EmployeePayroll.Create(1, new DateTime(2026, 7, 1), 3000m).Value;

        payrollRepository.GetByEmployeeAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<EmployeePayroll> { payroll });

        var handler = new GetPayrollQueryHandler(payrollRepository);
        var query = new GetPayrollQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].TotalAmount.Should().Be(3000m);
    }

    [Fact]
    public async Task Handle_WithNoPayrollsForEmployee_ShouldReturnEmptyList()
    {
        // Arrange
        var payrollRepository = Substitute.For<IEmployeePayrollRepository>();
        payrollRepository.GetByEmployeeAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<EmployeePayroll>());

        var handler = new GetPayrollQueryHandler(payrollRepository);
        var query = new GetPayrollQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
