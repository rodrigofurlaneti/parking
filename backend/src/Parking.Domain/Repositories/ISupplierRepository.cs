namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<Supplier>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(Supplier entity, CancellationToken ct = default);
}
