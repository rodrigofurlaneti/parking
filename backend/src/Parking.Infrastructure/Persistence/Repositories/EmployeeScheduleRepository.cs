namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class EmployeeScheduleRepository(AppDbContext context) : IEmployeeScheduleRepository
{
    public async Task<EmployeeSchedule?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.EmployeeSchedules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<EmployeeSchedule>> GetByEmployeeAsync(long employeeId, CancellationToken ct = default)
    {
        return await context.EmployeeSchedules.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(EmployeeSchedule entity, CancellationToken ct = default)
    {
        await context.EmployeeSchedules.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(EmployeeSchedule entity, CancellationToken ct = default)
    {
        context.EmployeeSchedules.Update(entity);
        await Task.CompletedTask;
    }
}
