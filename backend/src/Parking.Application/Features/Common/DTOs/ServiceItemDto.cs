namespace Parking.Application.Features.Common.DTOs;

public sealed record ServiceItemDto(
    long Id,
    long ServiceCategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal BaseCost,
    bool IsActive);
