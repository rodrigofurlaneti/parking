namespace Parking.Application.Features.Purchase.CreatePurchase;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record PurchaseItemInput(long ProductId, decimal QuantityOrdered, decimal UnitCost);

public sealed record CreatePurchaseCommand(
    long BranchId,
    long SupplierId,
    List<PurchaseItemInput> Items) : ICommand<PurchaseDto>;
