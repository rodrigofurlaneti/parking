namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class WashSessionRepository(AppDbContext context) : IWashSessionRepository
{
    public async Task<WashSession?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.WashSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<WashSession>> GetAllByScheduleAsync(long washScheduleId, CancellationToken ct = default)
    {
        return await context.WashSessions.AsNoTracking()
            .Where(x => x.WashScheduleId == washScheduleId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WashSession entity, CancellationToken ct = default)
    {
        await context.WashSessions.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(WashSession entity, CancellationToken ct = default)
    {
        context.WashSessions.Update(entity);
        await Task.CompletedTask;
    }
}
