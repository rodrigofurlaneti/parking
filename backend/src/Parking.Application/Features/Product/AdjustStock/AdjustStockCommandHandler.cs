namespace Parking.Application.Features.Product.AdjustStock;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class AdjustStockCommandHandler : ICommandHandler<AdjustStockCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockCommandHandler(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(new Error("Product.NotFound", "Product not found."));

        if (request.Adjustment == 0)
            return Result.Failure<ProductDto>(new Error("AdjustStock.InvalidAdjustment", "Adjustment must not be zero."));

        int movementType;

        if (request.Adjustment > 0)
        {
            product.IncreaseStock(request.Adjustment);
            movementType = StockMovement.AjustePositivo;
        }
        else
        {
            var decreaseResult = product.DecreaseStock(Math.Abs(request.Adjustment));
            if (decreaseResult.IsFailure)
                return Result.Failure<ProductDto>(decreaseResult.Error);

            movementType = StockMovement.AjusteNegativo;
        }

        var movementResult = StockMovement.Create(
            request.ProductId,
            movementType,
            Math.Abs(request.Adjustment),
            product.Cost,
            request.Reason,
            "Adjustment",
            null);

        if (movementResult.IsFailure)
            return Result.Failure<ProductDto>(movementResult.Error);

        await _stockMovementRepository.AddAsync(movementResult.Value, cancellationToken);
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ProductDto(
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
            product.IsActive));
    }
}
