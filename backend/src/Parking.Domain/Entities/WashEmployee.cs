namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class WashEmployee : AggregateRoot
{
    public long EmployeeId { get; private set; }
    public string? Specializations { get; private set; }
    public string? Certifications { get; private set; }
    public int TrainingLevel { get; private set; }
    public bool IsActive { get; private set; } = true;

    private WashEmployee() { }

    private WashEmployee(
        long employeeId,
        string? specializations,
        string? certifications,
        int trainingLevel) : base(0)
    {
        EmployeeId = employeeId;
        Specializations = specializations;
        Certifications = certifications;
        TrainingLevel = trainingLevel;
    }

    public static Result<WashEmployee> Create(
        long employeeId,
        string? specializations,
        string? certifications,
        int trainingLevel)
    {
        if (employeeId <= 0)
            return Result.Failure<WashEmployee>(new Error("WashEmployee.InvalidEmployee", "Employee is required."));

        if (trainingLevel < 0)
            return Result.Failure<WashEmployee>(new Error("WashEmployee.InvalidTrainingLevel", "Training level cannot be negative."));

        return Result.Success(new WashEmployee(employeeId, specializations, certifications, trainingLevel));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
