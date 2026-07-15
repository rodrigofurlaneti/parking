namespace Parking.Application.Features.Common.DTOs;

public sealed record CashMovementDto(
    long Id,
    long CashRegisterId,
    int Type,
    decimal Amount,
    string Description,
    int? ReferencedDocumentType,
    long? ReferencedDocumentId,
    bool IsActive);
