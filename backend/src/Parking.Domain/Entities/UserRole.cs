using Parking.Domain.Common;

namespace Parking.Domain.Entities;

public sealed class UserRole : Entity
{
    public long UserId { get; private set; }
    public long RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    private UserRole() { }

    public UserRole(long userId, long roleId) : base(0)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
    }
}
