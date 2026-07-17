namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class CompanyTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Company.Create("Acme", "12.345.678/0001-90", "Acme Legal Name");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Acme");
        result.Value.Cnpj.Should().Be("12.345.678/0001-90");
        result.Value.LegalName.Should().Be("Acme Legal Name");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = Company.Create("", "12.345.678/0001-90", "Acme Legal Name");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidName");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldFail()
    {
        var result = Company.Create("   ", "12.345.678/0001-90", "Acme Legal Name");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidName");
    }

    [Fact]
    public void Create_WithEmptyCnpj_ShouldFail()
    {
        var result = Company.Create("Acme", "", "Acme Legal Name");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Company.InvalidCnpj");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalseAndUpdatedAt()
    {
        var company = Company.Create("Acme", "12.345.678/0001-90", "Acme Legal Name").Value;

        company.Deactivate();

        company.IsActive.Should().BeFalse();
        company.UpdatedAt.Should().NotBeNull();
    }
}
