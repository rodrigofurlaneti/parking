namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Supplier : AggregateRoot
{
    public long BranchId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Document { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Supplier() { }

    private Supplier(
        long branchId,
        string name,
        string document,
        string? phone,
        string? email) : base(0)
    {
        BranchId = branchId;
        Name = name;
        Document = document;
        Phone = phone;
        Email = email;
    }

    public static Result<Supplier> Create(
        long branchId,
        string name,
        string document,
        string? phone,
        string? email)
    {
        if (branchId <= 0)
            return Result.Failure<Supplier>(new Error("Supplier.InvalidBranch", "Branch is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Supplier>(new Error("Supplier.InvalidName", "Name required."));

        if (string.IsNullOrWhiteSpace(document))
            return Result.Failure<Supplier>(new Error("Supplier.InvalidDocument", "Document required."));

        return Result.Success(new Supplier(branchId, name, document, phone, email));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
