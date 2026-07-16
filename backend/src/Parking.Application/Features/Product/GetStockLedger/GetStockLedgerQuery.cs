namespace Parking.Application.Features.Product.GetStockLedger;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetStockLedgerQuery(
    long ProductId,
    DateTime? FromDate,
    DateTime? ToDate) : IQuery<List<StockMovementDto>>;
