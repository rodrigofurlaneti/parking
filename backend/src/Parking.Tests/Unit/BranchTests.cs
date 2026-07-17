namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class BranchTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Branch.Create(1, "  Downtown  ", "  123 Main St  ", 50);

        result.IsSuccess.Should().BeTrue();
        result.Value.CompanyId.Should().Be(1);
        result.Value.Name.Should().Be("Downtown");
        result.Value.Address.Should().Be("123 Main St");
        result.Value.TotalSpaces.Should().Be(50);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidCompanyId_ShouldFail()
    {
        var result = Branch.Create(0, "Downtown", "123 Main St", 50);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidCompanyId");
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = Branch.Create(1, " ", "123 Main St", 50);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidName");
    }

    [Fact]
    public void Create_WithZeroTotalSpaces_ShouldFail()
    {
        var result = Branch.Create(1, "Downtown", "123 Main St", 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidTotalSpaces");
    }

    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var branch = Branch.Create(1, "Downtown", "123 Main St", 50).Value;

        var result = branch.Update("  Uptown  ", "  456 2nd Ave  ", "999999999", 80);

        result.IsSuccess.Should().BeTrue();
        branch.Name.Should().Be("Uptown");
        branch.Address.Should().Be("456 2nd Ave");
        branch.PhoneNumber.Should().Be("999999999");
        branch.TotalSpaces.Should().Be(80);
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var branch = Branch.Create(1, "Downtown", "123 Main St", 50).Value;

        var result = branch.Update(" ", "456 2nd Ave", null, 80);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidName");
    }

    [Fact]
    public void Update_WithZeroTotalSpaces_ShouldFail()
    {
        var branch = Branch.Create(1, "Downtown", "123 Main St", 50).Value;

        var result = branch.Update("Downtown", "456 2nd Ave", null, 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Branch.InvalidTotalSpaces");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var branch = Branch.Create(1, "Downtown", "123 Main St", 50).Value;

        branch.Deactivate();

        branch.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var branch = Branch.Create(1, "Downtown", "123 Main St", 50).Value;
        branch.Deactivate();

        branch.Activate();

        branch.IsActive.Should().BeTrue();
    }
}
