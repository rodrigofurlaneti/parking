namespace Parking.Application.Features.Purchase.ReceivePurchaseItems;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class ReceivePurchaseItemsCommandHandler : ICommandHandler<ReceivePurchaseItemsCommand, PurchaseDto>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPurchaseItemRepository _purchaseItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceivePurchaseItemsCommandHandler(
        IPurchaseRepository purchaseRepository,
        IPurchaseItemRepository purchaseItemRepository,
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _purchaseItemRepository = purchaseItemRepository;
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PurchaseDto>> Handle(ReceivePurchaseItemsCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.PurchaseId, cancellationToken);
        if (purchase is null)
            return Result.Failure<PurchaseDto>(new Error("Purchase.NotFound", "Purchase not found."));

        // The item/product updates and the purchase status update are split across two SaveChanges
        // calls, because the ledger query used to decide the final purchase status needs to read
        // back the quantities just persisted. Wrap both in a single ambient database transaction so
        // a failure in the second SaveChanges rolls back the first one too, avoiding a Purchase left
        // with partially-updated items/stock without a consistent status.
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var itemInput in request.Items)
            {
                var purchaseItem = await _purchaseItemRepository.GetByIdAsync(itemInput.PurchaseItemId, cancellationToken);
                if (purchaseItem is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<PurchaseDto>(new Error("PurchaseItem.NotFound", "Purchase item not found."));
                }

                if (purchaseItem.PurchaseId != request.PurchaseId)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<PurchaseDto>(
                        new Error("PurchaseItem.MismatchedPurchase", "Purchase item does not belong to this purchase."));
                }

                var receiveResult = purchaseItem.ReceiveQuantity(itemInput.QuantityReceived);
                if (receiveResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<PurchaseDto>(receiveResult.Error);
                }

                var product = await _productRepository.GetByIdAsync(purchaseItem.ProductId, cancellationToken);
                if (product is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<PurchaseDto>(new Error("Product.NotFound", "Product not found."));
                }

                product.IncreaseStock(itemInput.QuantityReceived);

                var movementResult = StockMovement.Create(
                    purchaseItem.ProductId,
                    StockMovement.CompraEntrada,
                    itemInput.QuantityReceived,
                    purchaseItem.UnitCost,
                    $"Recebimento da compra #{purchase.PurchaseNumber}",
                    "Purchase",
                    purchase.Id);

                if (movementResult.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure<PurchaseDto>(movementResult.Error);
                }

                await _stockMovementRepository.AddAsync(movementResult.Value, cancellationToken);
                await _purchaseItemRepository.UpdateAsync(purchaseItem, cancellationToken);
                await _productRepository.UpdateAsync(product, cancellationToken);
            }

            // Flush pending item/product updates so the ledger query below reflects
            // the quantities just received.
            await _unitOfWork.CommitAsync(cancellationToken);

            var allItems = await _purchaseItemRepository.GetByPurchaseAsync(request.PurchaseId, cancellationToken);
            if (allItems.All(x => x.IsFullyReceived))
                purchase.MarkAsReceived();
            else
                purchase.MarkAsPartiallyReceived();

            await _purchaseRepository.UpdateAsync(purchase, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var itemDtos = allItems.Select(x => new PurchaseItemDto(
                x.Id,
                x.PurchaseId,
                x.ProductId,
                x.QuantityOrdered,
                x.QuantityReceived,
                x.UnitCost,
                x.IsFullyReceived)).ToList();

            return Result.Success(new PurchaseDto(
                purchase.Id,
                purchase.BranchId,
                purchase.SupplierId,
                purchase.PurchaseNumber,
                purchase.PurchaseDate,
                purchase.Status,
                purchase.IsActive,
                itemDtos));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
