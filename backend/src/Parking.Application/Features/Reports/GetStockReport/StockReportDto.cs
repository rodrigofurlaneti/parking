namespace Parking.Application.Features.Reports.GetStockReport;

using Parking.Application.Features.Common.DTOs;

public sealed record StockReportDto(
    long BranchId,
    decimal TotalStockValue,
    IReadOnlyCollection<ProductDto> BelowMinimumProducts,
    IReadOnlyCollection<ProductMovementCountDto> TopMovedProducts);

public sealed record ProductMovementCountDto(long ProductId, string ProductName, int MovementCount);
