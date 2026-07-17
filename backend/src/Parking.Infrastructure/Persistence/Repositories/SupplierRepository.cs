namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class SupplierRepository(AppDbContext context) : ISupplierRepository
{
    public async Task<Supplier?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Suppliers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<Supplier>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Suppliers.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Supplier entity, CancellationToken ct = default)
    {
        await context.Suppliers.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Supplier entity, CancellationToken ct = default)
    {
        context.Suppliers.Update(entity);
        await Task.CompletedTask;
    }
}
