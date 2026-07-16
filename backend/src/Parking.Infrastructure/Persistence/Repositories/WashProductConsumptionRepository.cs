namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class WashProductConsumptionRepository(AppDbContext context) : IWashProductConsumptionRepository
{
    public async Task<WashProductConsumption?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.WashProductConsumptions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<WashProductConsumption>> GetAllByScheduleAsync(long washScheduleId, CancellationToken ct = default)
    {
        return await context.WashProductConsumptions.AsNoTracking()
            .Where(x => x.WashScheduleId == washScheduleId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WashProductConsumption entity, CancellationToken ct = default)
    {
        await context.WashProductConsumptions.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(WashProductConsumption entity, CancellationToken ct = default)
    {
        context.WashProductConsumptions.Update(entity);
        await Task.CompletedTask;
    }
}
