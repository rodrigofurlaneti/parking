namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IMonthlyCustomerContractRepository
{
    Task<List<MonthlyCustomerContract>> GetActiveByCustomerAsync(long customerId, CancellationToken ct = default);
    Task AddAsync(MonthlyCustomerContract entity, CancellationToken ct = default);
}
