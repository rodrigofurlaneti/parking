namespace Parking.Application.Features.Common.DTOs;

public sealed record VehicleDto(
    long Id,
    long CustomerId,
    string LicensePlate,
    string? Model,
    string? Color,
    bool IsActive);
