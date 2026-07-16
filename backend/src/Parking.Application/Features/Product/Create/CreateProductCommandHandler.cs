namespace Parking.Application.Features.Product.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainProduct = Parking.Domain.Entities.Product;

internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetBySKUAsync(request.BranchId, request.SKU, cancellationToken);
        if (existing is not null)
            return Result.Failure<ProductDto>(
                new Error("Product.DuplicateSKU", "Product with this SKU already exists for this branch."));

        var productResult = DomainProduct.Create(
            request.BranchId,
            request.Name,
            request.SKU,
            request.Category,
            request.Cost,
            request.SellingPrice,
            request.Stock,
            request.SupplierId);

        if (productResult.IsFailure)
            return Result.Failure<ProductDto>(productResult.Error);

        await _productRepository.AddAsync(productResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ProductDto(
            productResult.Value.Id,
            productResult.Value.BranchId,
            productResult.Value.Name,
            productResult.Value.SKU,
            productResult.Value.Category,
            productResult.Value.Cost,
            productResult.Value.SellingPrice,
            productResult.Value.Stock,
            productResult.Value.SupplierId,
            productResult.Value.IsActive));
    }
}
