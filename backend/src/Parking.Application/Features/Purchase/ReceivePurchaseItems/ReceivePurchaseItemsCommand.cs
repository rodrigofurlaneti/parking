namespace Parking.Application.Features.Purchase.ReceivePurchaseItems;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record ReceivePurchaseItemInput(long PurchaseItemId, decimal QuantityReceived);

public sealed record ReceivePurchaseItemsCommand(
    long PurchaseId,
    List<ReceivePurchaseItemInput> Items) : ICommand<PurchaseDto>;
