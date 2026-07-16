namespace Parking.Application.Features.Common.DTOs;

public sealed record PurchaseDto(
    long Id,
    long BranchId,
    long SupplierId,
    long PurchaseNumber,
    DateTime PurchaseDate,
    int Status,
    bool IsActive,
    IReadOnlyCollection<PurchaseItemDto> Items);
