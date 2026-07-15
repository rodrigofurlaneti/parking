namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Company : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string LegalName { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Company() { }

    private Company(long id, string name, string cnpj, string legalName) : base(id)
    {
        Name = name;
        Cnpj = cnpj;
        LegalName = legalName;
    }

    public static Result<Company> Create(string name, string cnpj, string legalName)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Company>(new Error("Company.InvalidName", "Name is required."));

        if (string.IsNullOrWhiteSpace(cnpj))
            return Result.Failure<Company>(new Error("Company.InvalidCnpj", "CNPJ is required."));

        return Result.Success(new Company(0, name.Trim(), cnpj.Trim(), legalName.Trim()));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
