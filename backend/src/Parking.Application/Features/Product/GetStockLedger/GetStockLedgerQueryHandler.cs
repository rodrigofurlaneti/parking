namespace Parking.Application.Features.Product.GetStockLedger;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetStockLedgerQueryHandler : IQueryHandler<GetStockLedgerQuery, List<StockMovementDto>>
{
    private readonly IStockMovementRepository _stockMovementRepository;

    public GetStockLedgerQueryHandler(IStockMovementRepository stockMovementRepository)
    {
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task<Result<List<StockMovementDto>>> Handle(GetStockLedgerQuery request, CancellationToken cancellationToken)
    {
        var movements = await _stockMovementRepository.GetByProductAsync(
            request.ProductId, request.FromDate, request.ToDate, cancellationToken);

        var dtos = movements.Select(x => new StockMovementDto(
            x.Id,
            x.ProductId,
            x.MovementType,
            x.Quantity,
            x.UnitCost,
            x.Reason,
            x.ReferencedDocumentType,
            x.ReferencedDocumentId,
            x.MovementDate)).ToList();

        return Result.Success(dtos);
    }
}
