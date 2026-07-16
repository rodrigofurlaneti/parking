namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class PurchaseRepository(AppDbContext context) : IPurchaseRepository
{
    public async Task<Purchase?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Purchases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<long> GetNextPurchaseNumberAsync(long branchId, CancellationToken ct = default)
    {
        var lastPurchaseNumber = await context.Purchases.AsNoTracking()
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.PurchaseNumber)
            .Select(x => x.PurchaseNumber)
            .FirstOrDefaultAsync(ct);

        return lastPurchaseNumber + 1;
    }

    public async Task AddAsync(Purchase entity, CancellationToken ct = default)
    {
        await context.Purchases.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Purchase entity, CancellationToken ct = default)
    {
        context.Purchases.Update(entity);
        await Task.CompletedTask;
    }
}
