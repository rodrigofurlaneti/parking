namespace Parking.Application.Features.Product.GetBelowMinimum;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetBelowMinimumQuery(long BranchId) : IQuery<List<ProductDto>>;
