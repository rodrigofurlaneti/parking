namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class WashScheduleRepository(AppDbContext context) : IWashScheduleRepository
{
    public async Task<WashSchedule?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.WashSchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<WashSchedule>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.WashSchedules.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<List<WashSchedule>> GetAllByEmployeeAsync(long employeeId, CancellationToken ct = default)
    {
        return await context.WashSchedules.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WashSchedule entity, CancellationToken ct = default)
    {
        await context.WashSchedules.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(WashSchedule entity, CancellationToken ct = default)
    {
        context.WashSchedules.Update(entity);
        await Task.CompletedTask;
    }
}
