namespace Parking.Tests.Handlers;

using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Parking.Application.Features.Reports.GetCashReport;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Xunit;

public sealed class GetCashReportQueryHandlerTests
{
    private static readonly PropertyInfo IdProperty = typeof(Entity).GetProperty(nameof(Entity.Id))!;

    private static CashRegister CreateClosedRegister(long id, long employeeId, decimal openingBalance, decimal closingBalance)
    {
        var register = CashRegister.Create(1, employeeId, openingBalance).Value;
        IdProperty.SetValue(register, id);
        register.Close(closingBalance);
        return register;
    }

    [Fact]
    public async Task Handle_WithMovements_ShouldReconcileAndSummarizeByOperator()
    {
        // Arrange
        var reportsRepository = Substitute.For<IReportsRepository>();
        var fromDate = new DateTime(2026, 7, 1);
        var toDate = new DateTime(2026, 7, 31);

        var register1 = CreateClosedRegister(1, 10, 100m, 140m); // expected = 100+50-20+5 = 135, diff = +5
        var register2 = CreateClosedRegister(2, 10, 200m, 245m); // expected = 200+100-50 = 250, diff = -5

        var registers = new List<CashRegister> { register1, register2 };

        var movements = new List<CashMovement>
        {
            CashMovement.Create(1, 1, 50m, "Entrada").Value,
            CashMovement.Create(1, 2, 20m, "Saida").Value,
            CashMovement.Create(1, 3, 5m, "Ajuste").Value,
            CashMovement.Create(2, 1, 100m, "Entrada").Value,
            CashMovement.Create(2, 2, 50m, "Saida").Value,
        };

        reportsRepository.GetClosedCashRegistersAsync(1, fromDate, toDate, Arg.Any<CancellationToken>()).Returns(registers);
        reportsRepository.GetCashMovementsAsync(Arg.Any<IReadOnlyCollection<long>>(), Arg.Any<CancellationToken>())
            .Returns(movements);

        var handler = new GetCashReportQueryHandler(reportsRepository);
        var query = new GetCashReportQuery(1, fromDate, toDate);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Reconciliations.Should().HaveCount(2);

        var reconciliation1 = result.Value.Reconciliations.Single(r => r.CashRegisterId == 1);
        reconciliation1.ExpectedAmount.Should().Be(135m);
        reconciliation1.Difference.Should().Be(5m);

        var reconciliation2 = result.Value.Reconciliations.Single(r => r.CashRegisterId == 2);
        reconciliation2.ExpectedAmount.Should().Be(250m);
        reconciliation2.Difference.Should().Be(-5m);

        result.Value.OperatorSummary.Should().ContainSingle();
        var operatorSummary = result.Value.OperatorSummary.Single();
        operatorSummary.EmployeeId.Should().Be(10);
        operatorSummary.RegistersOperated.Should().Be(2);
        operatorSummary.TotalDifference.Should().Be(0m);
    }
}
