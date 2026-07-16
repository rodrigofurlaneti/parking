namespace Parking.Application.Features.Product.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateProductCommand(
    long BranchId,
    string Name,
    string SKU,
    string Category,
    decimal Cost,
    decimal SellingPrice,
    decimal Stock,
    long? SupplierId) : ICommand<ProductDto>;
