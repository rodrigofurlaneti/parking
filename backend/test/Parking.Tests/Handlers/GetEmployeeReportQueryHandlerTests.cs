namespace Parking.Tests.Handlers;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Reports.GetEmployeeReport;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetEmployeeReportQueryHandlerTests
{
    private static readonly PropertyInfo IdProperty = typeof(Entity).GetProperty(nameof(Entity.Id))!;

    private static Employee CreateEmployee(long id, string name)
    {
        var employee = Employee.Create(1, 1, name, $"{name}@parking.com", "11999999999", "12345678901", 1).Value;
        IdProperty.SetValue(employee, id);
        return employee;
    }

    [Fact]
    public async Task Handle_WithSchedulesAndWashSessions_ShouldComputeProductivityCorrectly()
    {
        // Arrange
        var reportsRepository = Substitute.For<IReportsRepository>();

        // 7 consecutive days => every weekday (Monday..Sunday) occurs exactly once in the period.
        var fromDate = new DateTime(2026, 7, 6);
        var toDate = new DateTime(2026, 7, 12);

        var employee1 = CreateEmployee(1, "Joao");
        var employee2 = CreateEmployee(2, "Maria");
        var employees = new List<Employee> { employee1, employee2 };

        var schedules = new List<EmployeeSchedule>
        {
            EmployeeSchedule.Create(1, 0, new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)).Value,  // Monday, 9h
            EmployeeSchedule.Create(1, 2, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0)).Value,  // Wednesday, 4h
        };

        var washSessionCounts = new List<EmployeeWashSessionCount>
        {
            new(1, 3),
        };

        reportsRepository.GetActiveEmployeesAsync(1, Arg.Any<CancellationToken>()).Returns(employees);
        reportsRepository.GetEmployeeSchedulesAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(schedules);
        reportsRepository.GetCompletedWashSessionCountsByEmployeeAsync(1, fromDate, toDate, Arg.Any<CancellationToken>())
            .Returns(washSessionCounts);

        var handler = new GetEmployeeReportQueryHandler(reportsRepository);
        var query = new GetEmployeeReportQuery(1, fromDate, toDate);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Employees.Should().HaveCount(2);

        var joao = result.Value.Employees.Single(e => e.EmployeeId == 1);
        joao.HoursWorked.Should().Be(13m);
        joao.WashSessionsCompleted.Should().Be(3);

        var maria = result.Value.Employees.Single(e => e.EmployeeId == 2);
        maria.HoursWorked.Should().Be(0m);
        maria.WashSessionsCompleted.Should().Be(0);
    }
}
