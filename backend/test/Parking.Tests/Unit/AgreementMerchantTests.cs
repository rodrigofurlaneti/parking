namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class AgreementMerchantTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = AgreementMerchant.Create(1, "  Acme Corp  ", 10m);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.CompanyName.Should().Be("Acme Corp");
        result.Value.DiscountPercentage.Should().Be(10m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = AgreementMerchant.Create(1, "  ", 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NameRequired");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_WithInvalidDiscount_ShouldFail(decimal discount)
    {
        var result = AgreementMerchant.Create(1, "Acme", discount);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.InvalidDiscount");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Create_WithBoundaryValidDiscount_ShouldSucceed(decimal discount)
    {
        var result = AgreementMerchant.Create(1, "Acme", discount);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountPercentage.Should().Be(discount);
    }

    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var merchant = AgreementMerchant.Create(1, "Acme", 10m).Value;

        var result = merchant.Update("  Acme Corp  ", 20m);

        result.IsSuccess.Should().BeTrue();
        merchant.CompanyName.Should().Be("Acme Corp");
        merchant.DiscountPercentage.Should().Be(20m);
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var merchant = AgreementMerchant.Create(1, "Acme", 10m).Value;

        var result = merchant.Update(" ", 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.NameRequired");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Update_WithInvalidDiscount_ShouldFail(decimal discount)
    {
        var merchant = AgreementMerchant.Create(1, "Acme", 10m).Value;

        var result = merchant.Update("Acme", discount);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AgreementMerchant.InvalidDiscount");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var merchant = AgreementMerchant.Create(1, "Acme", 10m).Value;

        merchant.Deactivate();

        merchant.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var merchant = AgreementMerchant.Create(1, "Acme", 10m).Value;
        merchant.Deactivate();

        merchant.Activate();

        merchant.IsActive.Should().BeTrue();
    }
}
