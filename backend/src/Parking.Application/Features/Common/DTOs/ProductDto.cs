namespace Parking.Application.Features.Common.DTOs;

public sealed record ProductDto(
    long Id,
    long BranchId,
    string Name,
    string SKU,
    string Category,
    decimal Cost,
    decimal SellingPrice,
    decimal Stock,
    decimal MinimumStock,
    long? SupplierId,
    bool IsActive);
