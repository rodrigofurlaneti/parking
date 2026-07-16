namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class ServiceCategory : AggregateRoot
{
    public long BranchId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ServiceCategory() { }

    private ServiceCategory(long branchId, string name, string? description) : base(0)
    {
        BranchId = branchId;
        Name = name;
        Description = description;
    }

    public static Result<ServiceCategory> Create(long branchId, string name, string? description)
    {
        if (branchId <= 0)
            return Result.Failure<ServiceCategory>(new Error("ServiceCategory.InvalidBranch", "Branch is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<ServiceCategory>(new Error("ServiceCategory.InvalidName", "Name required."));

        return Result.Success(new ServiceCategory(branchId, name, description));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
