namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class WashSessionTests
{
    [Fact]
    public void Create_WithValidSchedule_ShouldSucceed()
    {
        var result = WashSession.Create(1, "Full wash");

        result.IsSuccess.Should().BeTrue();
        result.Value.WashScheduleId.Should().Be(1);
        result.Value.Notes.Should().Be("Full wash");
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithoutNotes_ShouldSucceedWithNullNotes()
    {
        var result = WashSession.Create(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Notes.Should().BeNull();
    }

    [Fact]
    public void Create_WithInvalidSchedule_ShouldFail()
    {
        var result = WashSession.Create(0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSession.InvalidSchedule");
    }

    [Fact]
    public void AddCost_ShouldAccumulateCostAndDuration()
    {
        var session = WashSession.Create(1).Value;

        session.AddCost(10m, 20);
        session.AddCost(5m, 10);

        session.TotalCost.Should().Be(15m);
        session.TotalDurationMinutes.Should().Be(30);
    }

    [Fact]
    public void Complete_WhenInProgress_ShouldSucceed()
    {
        var session = WashSession.Create(1).Value;

        var result = session.Complete();

        result.IsSuccess.Should().BeTrue();
        session.Status.Should().Be(1);
        session.EndTime.Should().NotBeNull();
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldFail()
    {
        var session = WashSession.Create(1).Value;
        session.Complete();

        var result = session.Complete();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSession.InvalidStatus");
    }

    [Fact]
    public void Cancel_WhenInProgress_ShouldSucceed()
    {
        var session = WashSession.Create(1).Value;

        var result = session.Cancel();

        result.IsSuccess.Should().BeTrue();
        session.Status.Should().Be(2);
    }

    [Fact]
    public void Cancel_WhenAlreadyCompleted_ShouldFail()
    {
        var session = WashSession.Create(1).Value;
        session.Complete();

        var result = session.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSession.InvalidStatus");
    }
}
