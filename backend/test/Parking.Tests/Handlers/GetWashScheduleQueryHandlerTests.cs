namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashSchedule.GetById;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetWashScheduleQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingSchedule_ShouldReturnDto()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var schedule = WashSchedule.Create(1, 1, DateTime.UtcNow, 1).Value;
        washScheduleRepository.GetByIdAsync(schedule.Id, Arg.Any<CancellationToken>()).Returns(schedule);

        var handler = new GetWashScheduleQueryHandler(washScheduleRepository);
        var query = new GetWashScheduleQuery(schedule.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.VehicleEntryId.Should().Be(1);
        result.Value.EmployeeId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithScheduleNotFound_ShouldFail()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        washScheduleRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((WashSchedule?)null);

        var handler = new GetWashScheduleQueryHandler(washScheduleRepository);
        var query = new GetWashScheduleQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashSchedule.NotFound");
    }
}
