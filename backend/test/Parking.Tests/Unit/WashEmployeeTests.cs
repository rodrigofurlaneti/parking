namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class WashEmployeeTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = WashEmployee.Create(1, "Waxing", "Certified Detailer", 2);

        result.IsSuccess.Should().BeTrue();
        result.Value.EmployeeId.Should().Be(1);
        result.Value.Specializations.Should().Be("Waxing");
        result.Value.Certifications.Should().Be("Certified Detailer");
        result.Value.TrainingLevel.Should().Be(2);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidEmployee_ShouldFail()
    {
        var result = WashEmployee.Create(0, null, null, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashEmployee.InvalidEmployee");
    }

    [Fact]
    public void Create_WithNegativeTrainingLevel_ShouldFail()
    {
        var result = WashEmployee.Create(1, null, null, -1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WashEmployee.InvalidTrainingLevel");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var employee = WashEmployee.Create(1, null, null, 1).Value;

        employee.Deactivate();

        employee.IsActive.Should().BeFalse();
    }
}
