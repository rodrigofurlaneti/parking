namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IAgreementCustomerContractRepository
{
    Task<List<AgreementCustomerContract>> GetActiveByCustomerAsync(long customerId, CancellationToken ct = default);
    Task AddAsync(AgreementCustomerContract entity, CancellationToken ct = default);
}
