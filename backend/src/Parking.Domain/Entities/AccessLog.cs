using Parking.Domain.Common;

namespace Parking.Domain.Entities;

public sealed class AccessLog : Entity
{
    public long UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string? Resource { get; private set; }
    public string? IpAddress { get; private set; }

    private AccessLog() { }

    public AccessLog(long userId, string action, string? resource = null, string? ipAddress = null) : base(0)
    {
        UserId = userId;
        Action = action;
        Resource = resource;
        IpAddress = ipAddress;
    }
}
