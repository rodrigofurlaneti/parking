namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class PurchaseItemRepository(AppDbContext context) : IPurchaseItemRepository
{
    public async Task<PurchaseItem?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.PurchaseItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<PurchaseItem>> GetByPurchaseAsync(long purchaseId, CancellationToken ct = default)
    {
        return await context.PurchaseItems.AsNoTracking()
            .Where(x => x.PurchaseId == purchaseId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(PurchaseItem entity, CancellationToken ct = default)
    {
        await context.PurchaseItems.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(PurchaseItem entity, CancellationToken ct = default)
    {
        context.PurchaseItems.Update(entity);
        await Task.CompletedTask;
    }
}
