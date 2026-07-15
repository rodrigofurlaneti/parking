namespace Parking.Application.Features.Common.DTOs;

public sealed record UserDto(
    long Id,
    string UserName,
    string Email,
    string FullName,
    bool IsActive,
    List<string> Roles);
