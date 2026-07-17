namespace Parking.Tests.Unit;

using System;
using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class MonthlyCustomerContractTests
{
    private static readonly DateTime Start = new(2026, 1, 1);
    private static readonly DateTime End = new(2026, 12, 31);

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = MonthlyCustomerContract.Create(1, 2, 200m, 3, Start, End);

        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(1);
        result.Value.BranchId.Should().Be(2);
        result.Value.MonthlyFee.Should().Be(200m);
        result.Value.MaxVehicles.Should().Be(3);
        result.Value.StartDate.Should().Be(Start);
        result.Value.EndDate.Should().Be(End);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithZeroFee_ShouldFail()
    {
        var result = MonthlyCustomerContract.Create(1, 2, 0m, 3, Start, End);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidFee");
    }

    [Fact]
    public void Create_WithNegativeFee_ShouldFail()
    {
        var result = MonthlyCustomerContract.Create(1, 2, -10m, 3, Start, End);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidFee");
    }

    [Fact]
    public void Create_WithZeroMaxVehicles_ShouldFail()
    {
        var result = MonthlyCustomerContract.Create(1, 2, 200m, 0, Start, End);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidMaxVehicles");
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldFail()
    {
        var result = MonthlyCustomerContract.Create(1, 2, 200m, 3, End, Start);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("MonthlyCustomerContract.InvalidPeriod");
    }

    [Fact]
    public void IsValidOn_WithinPeriodAndActive_ShouldReturnTrue()
    {
        var contract = MonthlyCustomerContract.Create(1, 2, 200m, 3, Start, End).Value;

        contract.IsValidOn(new DateTime(2026, 6, 1)).Should().BeTrue();
    }

    [Fact]
    public void IsValidOn_WhenDeactivated_ShouldReturnFalse()
    {
        var contract = MonthlyCustomerContract.Create(1, 2, 200m, 3, Start, End).Value;
        contract.Deactivate();

        contract.IsValidOn(new DateTime(2026, 6, 1)).Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var contract = MonthlyCustomerContract.Create(1, 2, 200m, 3, Start, End).Value;

        contract.Deactivate();

        contract.IsActive.Should().BeFalse();
    }
}
