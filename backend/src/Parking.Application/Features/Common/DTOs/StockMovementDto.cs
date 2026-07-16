namespace Parking.Application.Features.Common.DTOs;

public sealed record StockMovementDto(
    long Id,
    long ProductId,
    int MovementType,
    decimal Quantity,
    decimal UnitCost,
    string Reason,
    string? ReferencedDocumentType,
    long? ReferencedDocumentId,
    DateTime MovementDate);
