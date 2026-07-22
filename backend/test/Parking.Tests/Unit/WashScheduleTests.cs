namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class WashScheduleTests
{
    private static WashSchedule CreateSchedule() =>
        WashSchedule.Create(1, 2, DateTime.UtcNow.AddHours(1), 3).Value;

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var scheduledTime = DateTime.UtcNow.AddHours(1);

        var result = WashSchedule.Create(1, 2, scheduledTime, 3);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.VehicleEntryId.Should().Be(2);
        result.Value.ScheduledTime.Should().Be(scheduledTime);
        result.Value.EmployeeId.Should().Be(3);
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidBranch_ShouldFail()
    {
        var result = WashSchedule.Create(0, 2, DateTime.UtcNow, 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidBranch");
    }

    [Fact]
    public void Create_WithInvalidVehicleEntry_ShouldFail()
    {
        var result = WashSchedule.Create(1, 0, DateTime.UtcNow, 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidVehicleEntry");
    }

    [Fact]
    public void Create_WithInvalidEmployee_ShouldFail()
    {
        var result = WashSchedule.Create(1, 2, DateTime.UtcNow, 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidEmployee");
    }

    [Fact]
    public void AssignEmployee_WithValidEmployee_ShouldSucceed()
    {
        var schedule = CreateSchedule();

        var result = schedule.AssignEmployee(5);

        result.IsSuccess.Should().BeTrue();
        schedule.EmployeeId.Should().Be(5);
    }

    [Fact]
    public void AssignEmployee_WithInvalidEmployee_ShouldFail()
    {
        var schedule = CreateSchedule();

        var result = schedule.AssignEmployee(0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidEmployee");
    }

    [Fact]
    public void AssignEmployee_WhenScheduleCompleted_ShouldFail()
    {
        var schedule = CreateSchedule();
        schedule.Start();
        schedule.Complete();

        var result = schedule.AssignEmployee(5);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }

    [Fact]
    public void AssignEmployee_WhenScheduleCancelled_ShouldFail()
    {
        var schedule = CreateSchedule();
        schedule.Cancel();

        var result = schedule.AssignEmployee(5);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }

    [Fact]
    public void Start_WhenScheduled_ShouldSucceedAndSetStartTime()
    {
        var schedule = CreateSchedule();

        var result = schedule.Start();

        result.IsSuccess.Should().BeTrue();
        schedule.Status.Should().Be(1);
        schedule.ActualStartTime.Should().NotBeNull();
    }

    [Fact]
    public void Start_WhenAlreadyInProgress_ShouldFail()
    {
        var schedule = CreateSchedule();
        schedule.Start();

        var result = schedule.Start();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }

    [Fact]
    public void Complete_WhenInProgress_ShouldSucceedAndSetEndTime()
    {
        var schedule = CreateSchedule();
        schedule.Start();

        var result = schedule.Complete();

        result.IsSuccess.Should().BeTrue();
        schedule.Status.Should().Be(2);
        schedule.ActualEndTime.Should().NotBeNull();
    }

    [Fact]
    public void Complete_WhenNotInProgress_ShouldFail()
    {
        var schedule = CreateSchedule();

        var result = schedule.Complete();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }

    [Fact]
    public void Cancel_WhenScheduled_ShouldSucceed()
    {
        var schedule = CreateSchedule();

        var result = schedule.Cancel();

        result.IsSuccess.Should().BeTrue();
        schedule.Status.Should().Be(3);
    }

    [Fact]
    public void Cancel_WhenAlreadyCompleted_ShouldFail()
    {
        var schedule = CreateSchedule();
        schedule.Start();
        schedule.Complete();

        var result = schedule.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldFail()
    {
        var schedule = CreateSchedule();
        schedule.Cancel();

        var result = schedule.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.InvalidStatus");
    }
}
