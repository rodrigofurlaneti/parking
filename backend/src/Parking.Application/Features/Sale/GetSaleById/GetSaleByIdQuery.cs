namespace Parking.Application.Features.Sale.GetSaleById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetSaleByIdQuery(long Id) : IQuery<SaleDto>;
