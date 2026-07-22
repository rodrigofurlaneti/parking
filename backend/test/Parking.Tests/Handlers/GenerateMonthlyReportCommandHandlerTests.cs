namespace Parking.Tests.Handlers;

using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.WashOperationalCost.GenerateReport;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GenerateMonthlyReportCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldGenerateReport()
    {
        // Arrange
        var costRepository = Substitute.For<IWashOperationalCostRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        costRepository.GetByBranchAndMonthAsync(Arg.Any<long>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns((WashOperationalCost?)null);
        unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var handler = new GenerateMonthlyReportCommandHandler(costRepository, unitOfWork);
        var command = new GenerateMonthlyReportCommand(
            1, new DateTime(2026, 7, 1), 450m, 300m, 100m, 50m, 2250m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCost.Should().Be(900m);
        result.Value.NetProfit.Should().Be(1350m);
    }

    [Fact]
    public async Task Handle_WithExistingReport_ShouldFail()
    {
        // Arrange
        var costRepository = Substitute.For<IWashOperationalCostRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var existing = WashOperationalCost.Create(1, new DateTime(2026, 7, 1), 450m, 300m, 100m, 50m, 2250m).Value;
        costRepository.GetByBranchAndMonthAsync(Arg.Any<long>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(existing);

        var handler = new GenerateMonthlyReportCommandHandler(costRepository, unitOfWork);
        var command = new GenerateMonthlyReportCommand(
            1, new DateTime(2026, 7, 1), 450m, 300m, 100m, 50m, 2250m);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashOperationalCost.DuplicateReport");
    }
}
