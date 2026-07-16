namespace Parking.Application.Features.Reports.GetStockReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetStockReportQueryHandler : IQueryHandler<GetStockReportQuery, StockReportDto>
{
    private const int TopMovedProductsCount = 5;
    private const int RecentMovementsWindowDays = 30;

    private readonly IReportsRepository _reportsRepository;

    public GetStockReportQueryHandler(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<StockReportDto>> Handle(GetStockReportQuery request, CancellationToken cancellationToken)
    {
        var products = await _reportsRepository.GetActiveProductsAsync(request.BranchId, cancellationToken);

        var sinceDate = DateTime.UtcNow.AddDays(-RecentMovementsWindowDays);
        var movements = await _reportsRepository.GetRecentStockMovementsAsync(request.BranchId, sinceDate, cancellationToken);

        var totalStockValue = products.Sum(p => p.Stock * p.Cost);

        var belowMinimumProducts = products
            .Where(p => p.IsBelowMinimum())
            .Select(ToDto)
            .ToList();

        var productNamesById = products.ToDictionary(p => p.Id, p => p.Name);

        var topMovedProducts = movements
            .GroupBy(m => m.ProductId)
            .Select(g => new ProductMovementCountDto(
                g.Key,
                productNamesById.TryGetValue(g.Key, out var name) ? name : "N/A",
                g.Count()))
            .OrderByDescending(x => x.MovementCount)
            .Take(TopMovedProductsCount)
            .ToList();

        var dto = new StockReportDto(
            request.BranchId,
            totalStockValue,
            belowMinimumProducts,
            topMovedProducts);

        return Result.Success(dto);
    }

    private static ProductDto ToDto(Parking.Domain.Entities.Product product) => new(
        product.Id,
        product.BranchId,
        product.Name,
        product.SKU,
        product.Category,
        product.Cost,
        product.SellingPrice,
        product.Stock,
        product.MinimumStock,
        product.SupplierId,
        product.IsActive);
}
