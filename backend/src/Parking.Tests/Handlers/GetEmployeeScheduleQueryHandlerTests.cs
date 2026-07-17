namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Employee.GetEmployeeSchedule;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetEmployeeScheduleQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithSchedulesForEmployee_ShouldReturnDtos()
    {
        // Arrange
        var scheduleRepository = Substitute.For<IEmployeeScheduleRepository>();
        var schedule = EmployeeSchedule.Create(1, 1, new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0)).Value;

        scheduleRepository.GetByEmployeeAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<EmployeeSchedule> { schedule });

        var handler = new GetEmployeeScheduleQueryHandler(scheduleRepository);
        var query = new GetEmployeeScheduleQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].DayOfWeek.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNoSchedulesForEmployee_ShouldReturnEmptyList()
    {
        // Arrange
        var scheduleRepository = Substitute.For<IEmployeeScheduleRepository>();
        scheduleRepository.GetByEmployeeAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<EmployeeSchedule>());

        var handler = new GetEmployeeScheduleQueryHandler(scheduleRepository);
        var query = new GetEmployeeScheduleQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
