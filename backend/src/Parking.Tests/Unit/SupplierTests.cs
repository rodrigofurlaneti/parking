namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class SupplierTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Supplier.Create(1, "Acme", "12345678000199", "999999999", "acme@test.com");

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.Name.Should().Be("Acme");
        result.Value.Document.Should().Be("12345678000199");
        result.Value.Phone.Should().Be("999999999");
        result.Value.Email.Should().Be("acme@test.com");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidBranch_ShouldFail()
    {
        var result = Supplier.Create(0, "Acme", "123", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidBranch");
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = Supplier.Create(1, "", "123", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidName");
    }

    [Fact]
    public void Create_WithMissingDocument_ShouldFail()
    {
        var result = Supplier.Create(1, "Acme", " ", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidDocument");
    }

    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var supplier = Supplier.Create(1, "Acme", "123", null, null).Value;

        var result = supplier.Update("Acme Corp", "456", "111", "acme@corp.com");

        result.IsSuccess.Should().BeTrue();
        supplier.Name.Should().Be("Acme Corp");
        supplier.Document.Should().Be("456");
        supplier.Phone.Should().Be("111");
        supplier.Email.Should().Be("acme@corp.com");
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var supplier = Supplier.Create(1, "Acme", "123", null, null).Value;

        var result = supplier.Update("", "456", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidName");
    }

    [Fact]
    public void Update_WithMissingDocument_ShouldFail()
    {
        var supplier = Supplier.Create(1, "Acme", "123", null, null).Value;

        var result = supplier.Update("Acme", "  ", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Supplier.InvalidDocument");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var supplier = Supplier.Create(1, "Acme", "123", null, null).Value;

        supplier.Deactivate();

        supplier.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var supplier = Supplier.Create(1, "Acme", "123", null, null).Value;
        supplier.Deactivate();

        supplier.Activate();

        supplier.IsActive.Should().BeTrue();
    }
}
