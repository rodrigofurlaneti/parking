namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IAgreementMerchantRepository
{
    Task<AgreementMerchant?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<AgreementMerchant>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(AgreementMerchant entity, CancellationToken ct = default);
    Task UpdateAsync(AgreementMerchant entity, CancellationToken ct = default);
}
