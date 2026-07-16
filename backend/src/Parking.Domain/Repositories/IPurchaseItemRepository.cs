namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IPurchaseItemRepository
{
    Task<PurchaseItem?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<PurchaseItem>> GetByPurchaseAsync(long purchaseId, CancellationToken ct = default);
    Task AddAsync(PurchaseItem entity, CancellationToken ct = default);
    Task UpdateAsync(PurchaseItem entity, CancellationToken ct = default);
}
