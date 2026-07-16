namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class ServiceCategoryRepository(AppDbContext context) : IServiceCategoryRepository
{
    public async Task<ServiceCategory?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.ServiceCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<ServiceCategory>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.ServiceCategories.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ServiceCategory entity, CancellationToken ct = default)
    {
        await context.ServiceCategories.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(ServiceCategory entity, CancellationToken ct = default)
    {
        context.ServiceCategories.Update(entity);
        await Task.CompletedTask;
    }
}
