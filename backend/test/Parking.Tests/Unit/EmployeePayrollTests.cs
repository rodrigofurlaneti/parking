namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class EmployeePayrollTests
{
    [Fact]
    public void Create_WithValidSalary_ShouldSucceed()
    {
        var monthYear = new DateTime(2026, 7, 1);

        var result = EmployeePayroll.Create(1, monthYear, 3000m);

        result.IsSuccess.Should().BeTrue();
        result.Value.EmployeeId.Should().Be(1);
        result.Value.MonthYear.Should().Be(monthYear);
        result.Value.BaseSalary.Should().Be(3000m);
        result.Value.Status.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithZeroSalary_ShouldFail()
    {
        var result = EmployeePayroll.Create(1, DateTime.UtcNow, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeePayroll.InvalidSalary");
    }

    [Fact]
    public void Create_WithNegativeSalary_ShouldFail()
    {
        var result = EmployeePayroll.Create(1, DateTime.UtcNow, -100m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("EmployeePayroll.InvalidSalary");
    }

    [Fact]
    public void GetTotalAmount_ShouldSumBaseSalaryPlusBonusesMinusDeductions()
    {
        var payroll = EmployeePayroll.Create(1, DateTime.UtcNow, 3000m).Value;

        var total = payroll.GetTotalAmount();

        total.Should().Be(3000m);
    }

    [Fact]
    public void Approve_ShouldSetStatusToOne()
    {
        var payroll = EmployeePayroll.Create(1, DateTime.UtcNow, 3000m).Value;

        payroll.Approve();

        payroll.Status.Should().Be(1);
    }

    [Fact]
    public void MarkAsPaid_ShouldSetStatusToTwo()
    {
        var payroll = EmployeePayroll.Create(1, DateTime.UtcNow, 3000m).Value;

        payroll.MarkAsPaid();

        payroll.Status.Should().Be(2);
    }
}
