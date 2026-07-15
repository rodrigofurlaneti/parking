namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Permission : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Permission() { }

    private Permission(string name, string? description) : base(0)
    {
        Name = name;
        Description = description;
    }

    public static Result<Permission> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Permission>(new Error("Permission.InvalidName", "Name is required."));

        return Result.Success(new Permission(name.Trim(), description?.Trim()));
    }
}
