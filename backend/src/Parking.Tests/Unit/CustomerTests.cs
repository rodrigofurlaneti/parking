namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class CustomerTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Customer.Create(1, 1, "John Doe", "12345678900", "999999999", "john@test.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.CustomerType.Should().Be(1);
        result.Value.Name.Should().Be("John Doe");
        result.Value.Document.Should().Be("12345678900");
        result.Value.Phone.Should().Be("999999999");
        result.Value.Email.Should().Be("john@test.com");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldTrimNameAndDocument()
    {
        var result = Customer.Create(1, 1, "  John Doe  ", "  123  ", null, null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("John Doe");
        result.Value.Document.Should().Be("123");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void Create_WithInvalidCustomerType_ShouldFail(int type)
    {
        var result = Customer.Create(1, type, "John Doe", "123", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.InvalidType");
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = Customer.Create(1, 1, "", "123", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.NameRequired");
    }

    [Fact]
    public void Create_WithMissingDocument_ShouldFail()
    {
        var result = Customer.Create(1, 1, "John Doe", "  ", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Customer.DocumentRequired");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var customer = Customer.Create(1, 1, "John Doe", "123", null, null).Value;

        customer.Deactivate();

        customer.IsActive.Should().BeFalse();
    }
}
