namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class TariffRepository(AppDbContext context) : ITariffRepository
{
    public async Task<Tariff?> GetActiveByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Tariffs
            .AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Tariff?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Tariffs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<Tariff>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Tariffs.AsNoTracking()
            .Where(x => x.BranchId == branchId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Tariff entity, CancellationToken ct = default)
    {
        await context.Tariffs.AddAsync(entity, ct);
    }

    public Task UpdateAsync(Tariff entity, CancellationToken ct = default)
    {
        context.Tariffs.Update(entity);
        return Task.CompletedTask;
    }
}
