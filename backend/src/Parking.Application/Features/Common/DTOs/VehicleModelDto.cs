namespace Parking.Application.Features.Common.DTOs;

public sealed record VehicleModelDto(
    long Id,
    string Name,
    bool IsActive);
