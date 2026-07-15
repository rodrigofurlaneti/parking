using Parking.Domain.Common;

namespace Parking.Domain.Entities;

public sealed class RolePermission : Entity
{
    public long RoleId { get; private set; }
    public long PermissionId { get; private set; }

    private RolePermission() { }

    public RolePermission(long roleId, long permissionId) : base(0)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
