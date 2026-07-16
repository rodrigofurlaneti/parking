namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Product?> GetBySKUAsync(long branchId, string sku, CancellationToken ct = default);
    Task<List<Product>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(Product entity, CancellationToken ct = default);
    Task UpdateAsync(Product entity, CancellationToken ct = default);
}
