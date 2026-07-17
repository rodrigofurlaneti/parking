namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Product?> GetBySKUAsync(long branchId, string sku, CancellationToken ct = default)
    {
        return await context.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BranchId == branchId && x.SKU == sku, ct);
    }

    public async Task<List<Product>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Products.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product entity, CancellationToken ct = default)
    {
        await context.Products.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Product entity, CancellationToken ct = default)
    {
        context.Products.Update(entity);
        await Task.CompletedTask;
    }
}
