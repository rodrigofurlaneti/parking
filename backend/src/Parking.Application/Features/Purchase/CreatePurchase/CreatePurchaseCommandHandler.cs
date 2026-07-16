namespace Parking.Application.Features.Purchase.CreatePurchase;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using DomainPurchase = Parking.Domain.Entities.Purchase;

internal sealed class CreatePurchaseCommandHandler : ICommandHandler<CreatePurchaseCommand, PurchaseDto>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPurchaseItemRepository _purchaseItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseCommandHandler(
        ISupplierRepository supplierRepository,
        IPurchaseRepository purchaseRepository,
        IPurchaseItemRepository purchaseItemRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _purchaseRepository = purchaseRepository;
        _purchaseItemRepository = purchaseItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PurchaseDto>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return Result.Failure<PurchaseDto>(new Error("Supplier.NotFound", "Supplier not found."));

        var purchaseNumber = await _purchaseRepository.GetNextPurchaseNumberAsync(request.BranchId, cancellationToken);

        var purchaseResult = DomainPurchase.Create(request.BranchId, request.SupplierId, purchaseNumber);
        if (purchaseResult.IsFailure)
            return Result.Failure<PurchaseDto>(purchaseResult.Error);

        var purchase = purchaseResult.Value;
        await _purchaseRepository.AddAsync(purchase, cancellationToken);

        // Flush the insert so the DB-generated identity (purchase.Id) is available
        // to build the PurchaseItem rows that reference it by FK value.
        await _unitOfWork.CommitAsync(cancellationToken);

        var itemDtos = new List<PurchaseItemDto>();
        foreach (var itemInput in request.Items)
        {
            var itemResult = PurchaseItem.Create(
                purchase.Id, itemInput.ProductId, itemInput.QuantityOrdered, itemInput.UnitCost);

            if (itemResult.IsFailure)
                return Result.Failure<PurchaseDto>(itemResult.Error);

            await _purchaseItemRepository.AddAsync(itemResult.Value, cancellationToken);
            itemDtos.Add(new PurchaseItemDto(
                itemResult.Value.Id,
                itemResult.Value.PurchaseId,
                itemResult.Value.ProductId,
                itemResult.Value.QuantityOrdered,
                itemResult.Value.QuantityReceived,
                itemResult.Value.UnitCost,
                itemResult.Value.IsFullyReceived));
        }

        await _unitOfWork.CommitAsync(cancellationToken);

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
}
