namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Employee : AggregateRoot
{
    public long CompanyId { get; private set; }
    public long BranchId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string CPF { get; private set; } = null!;
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public long RoleId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Employee() { }

    private Employee(
        long companyId,
        long branchId,
        string name,
        string email,
        string phone,
        string cpf,
        long roleId) : base(0)
    {
        CompanyId = companyId;
        BranchId = branchId;
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        HireDate = DateTime.UtcNow;
        RoleId = roleId;
    }

    public static Result<Employee> Create(
        long companyId,
        long branchId,
        string name,
        string email,
        string phone,
        string cpf,
        long roleId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Employee>(new Error("Employee.InvalidName", "Name required."));

        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            return Result.Failure<Employee>(new Error("Employee.InvalidCPF", "Valid CPF required."));

        return Result.Success(new Employee(companyId, branchId, name, email, phone, cpf, roleId));
    }

    public void Terminate()
    {
        TerminationDate = DateTime.UtcNow;
        IsActive = false;
    }
}
