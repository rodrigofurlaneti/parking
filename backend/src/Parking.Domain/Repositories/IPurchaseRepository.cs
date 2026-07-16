namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<long> GetNextPurchaseNumberAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(Purchase entity, CancellationToken ct = default);
    Task UpdateAsync(Purchase entity, CancellationToken ct = default);
}
