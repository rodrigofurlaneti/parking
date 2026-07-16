namespace Parking.Application.Features.Product.GetBelowMinimum;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetBelowMinimumQueryHandler : IQueryHandler<GetBelowMinimumQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetBelowMinimumQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<List<ProductDto>>> Handle(GetBelowMinimumQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = products
            .Where(x => x.IsBelowMinimum())
            .Select(x => new ProductDto(
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
                x.IsActive))
            .ToList();

        return Result.Success(dtos);
    }
}
