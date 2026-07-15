namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Branch : AggregateRoot
{
    public long CompanyId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public int TotalSpaces { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Branch() { }

    private Branch(long companyId, string name, string address, int totalSpaces) : base(0)
    {
        CompanyId = companyId;
        Name = name;
        Address = address;
        TotalSpaces = totalSpaces;
    }

    public static Result<Branch> Create(long companyId, string name, string address, int totalSpaces)
    {
        if (companyId <= 0)
            return Result.Failure<Branch>(new Error("Branch.InvalidCompanyId", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Branch>(new Error("Branch.InvalidName", "Name is required."));

        if (totalSpaces <= 0)
            return Result.Failure<Branch>(new Error("Branch.InvalidTotalSpaces", "Total spaces must be greater than 0."));

        return Result.Success(new Branch(companyId, name.Trim(), address.Trim(), totalSpaces));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
