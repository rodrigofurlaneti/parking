namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Role : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Role() { }

    private Role(string name, string? description) : base(0)
    {
        Name = name;
        Description = description;
    }

    public static Result<Role> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Role>(new Error("Role.InvalidName", "Name is required."));

        return Result.Success(new Role(name.Trim(), description?.Trim()));
    }
}
