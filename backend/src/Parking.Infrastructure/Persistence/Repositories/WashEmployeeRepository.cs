namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class WashEmployeeRepository(AppDbContext context) : IWashEmployeeRepository
{
    public async Task<WashEmployee?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.WashEmployees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<WashEmployee?> GetByEmployeeIdAsync(long employeeId, CancellationToken ct = default)
    {
        return await context.WashEmployees.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId, ct);
    }

    public async Task<List<WashEmployee>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await context.WashEmployees.AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WashEmployee entity, CancellationToken ct = default)
    {
        await context.WashEmployees.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(WashEmployee entity, CancellationToken ct = default)
    {
        context.WashEmployees.Update(entity);
        await Task.CompletedTask;
    }
}
