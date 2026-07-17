namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class EmployeeTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "12345678900", 3);

        result.IsSuccess.Should().BeTrue();
        result.Value.CompanyId.Should().Be(1);
        result.Value.BranchId.Should().Be(2);
        result.Value.Name.Should().Be("John Doe");
        result.Value.Email.Should().Be("john@test.com");
        result.Value.Phone.Should().Be("999999999");
        result.Value.CPF.Should().Be("12345678900");
        result.Value.RoleId.Should().Be(3);
        result.Value.IsActive.Should().BeTrue();
        result.Value.TerminationDate.Should().BeNull();
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = Employee.Create(1, 2, "", "john@test.com", "999999999", "12345678900", 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.InvalidName");
    }

    [Fact]
    public void Create_WithMissingCPF_ShouldFail()
    {
        var result = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "", 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.InvalidCPF");
    }

    [Fact]
    public void Create_WithShortCPF_ShouldFail()
    {
        var result = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "123", 3);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.InvalidCPF");
    }

    [Fact]
    public void Update_WithValidData_ShouldSucceed()
    {
        var employee = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "12345678900", 3).Value;

        var result = employee.Update("Jane Doe", "jane@test.com", "888888888", 4);

        result.IsSuccess.Should().BeTrue();
        employee.Name.Should().Be("Jane Doe");
        employee.Email.Should().Be("jane@test.com");
        employee.Phone.Should().Be("888888888");
        employee.RoleId.Should().Be(4);
    }

    [Fact]
    public void Update_WithMissingName_ShouldFail()
    {
        var employee = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "12345678900", 3).Value;

        var result = employee.Update("", "jane@test.com", "888888888", 4);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Employee.InvalidName");
    }

    [Fact]
    public void Terminate_ShouldSetTerminationDateAndDeactivate()
    {
        var employee = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "12345678900", 3).Value;

        employee.Terminate();

        employee.IsActive.Should().BeFalse();
        employee.TerminationDate.Should().NotBeNull();
    }

    [Fact]
    public void Reactivate_ShouldClearTerminationDateAndActivate()
    {
        var employee = Employee.Create(1, 2, "John Doe", "john@test.com", "999999999", "12345678900", 3).Value;
        employee.Terminate();

        employee.Reactivate();

        employee.IsActive.Should().BeTrue();
        employee.TerminationDate.Should().BeNull();
    }
}
