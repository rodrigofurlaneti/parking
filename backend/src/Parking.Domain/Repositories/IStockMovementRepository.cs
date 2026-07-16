namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IStockMovementRepository
{
    Task AddAsync(StockMovement entity, CancellationToken ct = default);

    Task<List<StockMovement>> GetByProductAsync(
        long productId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default);
}
