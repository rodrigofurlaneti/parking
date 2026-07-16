namespace Parking.Application.Features.Product.GetStock;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetProductStockQueryHandler : IQueryHandler<GetProductStockQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductStockQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<List<ProductDto>>> Handle(GetProductStockQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = products.Select(x => new ProductDto(
            x.Id,
            x.BranchId,
            x.Name,
            x.SKU,
            x.Category,
            x.Cost,
            x.SellingPrice,
            x.Stock,
            x.MinimumStock,
            x.SupplierId,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
