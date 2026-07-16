namespace Parking.Application.Features.Common.DTOs;

public sealed record SupplierDto(
    long Id,
    long BranchId,
    string Name,
    string Document,
    string? Phone,
    string? Email,
    bool IsActive);
