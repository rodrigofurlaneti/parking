namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class EmployeeScheduleTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = EmployeeSchedule.Create(1, 0, TimeSpan.FromHours(8), TimeSpan.FromHours(17));

        result.IsSuccess.Should().BeTrue();
        result.Value.EmployeeId.Should().Be(1);
        result.Value.DayOfWeek.Should().Be(0);
        result.Value.StartTime.Should().Be(TimeSpan.FromHours(8));
        result.Value.EndTime.Should().Be(TimeSpan.FromHours(17));
        result.Value.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    public void Create_WithInvalidDayOfWeek_ShouldFail(int dayOfWeek)
    {
        var result = EmployeeSchedule.Create(1, dayOfWeek, TimeSpan.FromHours(8), TimeSpan.FromHours(17));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeeSchedule.InvalidDay");
    }

    [Fact]
    public void Create_WithStartTimeEqualToEndTime_ShouldFail()
    {
        var result = EmployeeSchedule.Create(1, 0, TimeSpan.FromHours(8), TimeSpan.FromHours(8));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeeSchedule.InvalidTime");
    }

    [Fact]
    public void Create_WithStartTimeAfterEndTime_ShouldFail()
    {
        var result = EmployeeSchedule.Create(1, 0, TimeSpan.FromHours(18), TimeSpan.FromHours(8));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeeSchedule.InvalidTime");
    }
}
