namespace Parking.Application.Features.Common.DTOs;

public sealed record CustomerDto(
    long Id,
    long BranchId,
    int CustomerType,
    string Name,
    string Document,
    string? Phone,
    string? Email,
    bool IsActive);
