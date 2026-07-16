namespace Parking.Application.Features.Common.DTOs;

public sealed record SaleDto(
    long Id,
    long BranchId,
    long VehicleExitId,
    long SaleNumber,
    decimal TotalAmount,
    DateTime SaleDate,
    bool IsActive,
    IReadOnlyCollection<SalePaymentDto> Payments);
