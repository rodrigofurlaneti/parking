namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class WashServiceRevenueRepository(AppDbContext context) : IWashServiceRevenueRepository
{
    public async Task<WashServiceRevenue?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.WashServiceRevenues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<WashServiceRevenue>> GetAllByScheduleAsync(long washScheduleId, CancellationToken ct = default)
    {
        return await context.WashServiceRevenues.AsNoTracking()
            .Where(x => x.WashScheduleId == washScheduleId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<List<WashServiceRevenue>> GetAllByPeriodAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await (from r in context.WashServiceRevenues.AsNoTracking()
                       join s in context.WashSchedules.AsNoTracking() on r.WashScheduleId equals s.Id
                       where s.BranchId == branchId && r.Date >= fromDate && r.Date <= toDate && r.IsActive
                       select r).ToListAsync(ct);
    }

    public async Task AddAsync(WashServiceRevenue entity, CancellationToken ct = default)
    {
        await context.WashServiceRevenues.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(WashServiceRevenue entity, CancellationToken ct = default)
    {
        context.WashServiceRevenues.Update(entity);
        await Task.CompletedTask;
    }
}
