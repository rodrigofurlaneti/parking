namespace Parking.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Parking.Application.Abstractions.Services;

internal sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUser
{
    public long? Id
    {
        get
        {
            var claim = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(claim, out var id) ? id : null;
        }
    }

    public string? UserName => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
