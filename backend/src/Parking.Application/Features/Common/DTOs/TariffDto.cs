namespace Parking.Application.Features.Common.DTOs;

public sealed record TariffDto(
    long Id,
    long BranchId,
    decimal FirstHourRate,
    decimal AdditionalHourRate,
    decimal? DailyMaxRate,
    bool IsActive);
