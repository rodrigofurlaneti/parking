namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IAgreementMerchantRepository
{
    Task<AgreementMerchant?> GetByIdAsync(long id, CancellationToken ct = default);
    Task AddAsync(AgreementMerchant entity, CancellationToken ct = default);
}
