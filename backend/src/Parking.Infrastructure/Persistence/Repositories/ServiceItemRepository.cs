namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class ServiceItemRepository(AppDbContext context) : IServiceItemRepository
{
    public async Task<ServiceItem?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.ServiceItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<ServiceItem>> GetAllByCategoryAsync(long serviceCategoryId, CancellationToken ct = default)
    {
        return await context.ServiceItems.AsNoTracking()
            .Where(x => x.ServiceCategoryId == serviceCategoryId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ServiceItem entity, CancellationToken ct = default)
    {
        await context.ServiceItems.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(ServiceItem entity, CancellationToken ct = default)
    {
        context.ServiceItems.Update(entity);
        await Task.CompletedTask;
    }
}
