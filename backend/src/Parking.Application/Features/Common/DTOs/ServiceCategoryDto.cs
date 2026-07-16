namespace Parking.Application.Features.Common.DTOs;

public sealed record ServiceCategoryDto(
    long Id,
    long BranchId,
    string Name,
    string? Description,
    bool IsActive);
