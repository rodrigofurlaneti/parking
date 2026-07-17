namespace Parking.Tests.Unit;

using System;
using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class AgreementCustomerContractTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 12, 31);

        var result = AgreementCustomerContract.Create(1, 2, start, end);

        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(1);
        result.Value.AgreementMerchantId.Should().Be(2);
        result.Value.StartDate.Should().Be(start);
        result.Value.EndDate.Should().Be(end);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEndDateEqualToStartDate_ShouldFail()
    {
        var date = new DateTime(2026, 1, 1);

        var result = AgreementCustomerContract.Create(1, 2, date, date);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementCustomerContract.InvalidPeriod");
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldFail()
    {
        var start = new DateTime(2026, 1, 10);
        var end = new DateTime(2026, 1, 1);

        var result = AgreementCustomerContract.Create(1, 2, start, end);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementCustomerContract.InvalidPeriod");
    }

    [Fact]
    public void IsValidOn_WithinPeriodAndActive_ShouldReturnTrue()
    {
        var contract = AgreementCustomerContract.Create(1, 2, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)).Value;

        contract.IsValidOn(new DateTime(2026, 6, 1)).Should().BeTrue();
    }

    [Fact]
    public void IsValidOn_OutsidePeriod_ShouldReturnFalse()
    {
        var contract = AgreementCustomerContract.Create(1, 2, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)).Value;

        contract.IsValidOn(new DateTime(2027, 1, 1)).Should().BeFalse();
    }

    [Fact]
    public void IsValidOn_WhenDeactivated_ShouldReturnFalse()
    {
        var contract = AgreementCustomerContract.Create(1, 2, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)).Value;
        contract.Deactivate();

        contract.IsValidOn(new DateTime(2026, 6, 1)).Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var contract = AgreementCustomerContract.Create(1, 2, new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)).Value;

        contract.Deactivate();

        contract.IsActive.Should().BeFalse();
    }
}
