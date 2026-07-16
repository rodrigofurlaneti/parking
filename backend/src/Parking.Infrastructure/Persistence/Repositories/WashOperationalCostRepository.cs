namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class WashOperationalCostRepository(AppDbContext context) : IWashOperationalCostRepository
{
    public async Task<WashOperationalCost?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.WashOperationalCosts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<WashOperationalCost?> GetByBranchAndMonthAsync(long branchId, DateTime monthYear, CancellationToken ct = default)
    {
        return await context.WashOperationalCosts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BranchId == branchId
                && x.MonthYear.Year == monthYear.Year
                && x.MonthYear.Month == monthYear.Month, ct);
    }

    public async Task<List<WashOperationalCost>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.WashOperationalCosts.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WashOperationalCost entity, CancellationToken ct = default)
    {
        await context.WashOperationalCosts.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(WashOperationalCost entity, CancellationToken ct = default)
    {
        context.WashOperationalCosts.Update(entity);
        await Task.CompletedTask;
    }
}
