namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class StockMovementRepository(AppDbContext context) : IStockMovementRepository
{
    public async Task AddAsync(StockMovement entity, CancellationToken ct = default)
    {
        await context.StockMovements.AddAsync(entity, ct);
    }

    public async Task<List<StockMovement>> GetByProductAsync(
        long productId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default)
    {
        var query = context.StockMovements.AsNoTracking().Where(x => x.ProductId == productId);

        if (fromDate.HasValue)
            query = query.Where(x => x.MovementDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.MovementDate <= toDate.Value);

        return await query.OrderBy(x => x.MovementDate).ToListAsync(ct);
    }
}
