namespace Parking.Application.Features.Product.AdjustStock;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record AdjustStockCommand(
    long ProductId,
    decimal Adjustment,
    string Reason) : ICommand<ProductDto>;
