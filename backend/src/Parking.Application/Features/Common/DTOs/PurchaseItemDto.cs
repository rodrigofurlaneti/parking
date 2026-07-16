namespace Parking.Application.Features.Common.DTOs;

public sealed record PurchaseItemDto(
    long Id,
    long PurchaseId,
    long ProductId,
    decimal QuantityOrdered,
    decimal QuantityReceived,
    decimal UnitCost,
    bool IsFullyReceived);
