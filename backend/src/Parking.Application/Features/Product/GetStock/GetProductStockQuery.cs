namespace Parking.Application.Features.Product.GetStock;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetProductStockQuery(long BranchId) : IQuery<List<ProductDto>>;
