namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashSchedule.GetAllByBranch;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetAllWashSchedulesByBranchQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithSchedulesInBranch_ShouldReturnDtos()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        var schedule1 = WashSchedule.Create(1, 1, DateTime.UtcNow, 1).Value;
        var schedule2 = WashSchedule.Create(1, 2, DateTime.UtcNow, 2).Value;

        washScheduleRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<WashSchedule> { schedule1, schedule2 });

        var handler = new GetAllWashSchedulesByBranchQueryHandler(washScheduleRepository);
        var query = new GetAllWashSchedulesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoSchedulesInBranch_ShouldReturnEmptyList()
    {
        // Arrange
        var washScheduleRepository = Substitute.For<IWashScheduleRepository>();
        washScheduleRepository.GetAllByBranchAsync(1, Arg.Any<CancellationToken>())
            .Returns(new List<WashSchedule>());

        var handler = new GetAllWashSchedulesByBranchQueryHandler(washScheduleRepository);
        var query = new GetAllWashSchedulesByBranchQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
