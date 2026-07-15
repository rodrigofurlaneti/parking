namespace Parking.Application.Features.Common.DTOs;

public sealed record RoleDto(
    long Id,
    string Name,
    bool IsActive,
    List<string> Permissions);
