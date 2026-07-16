namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IWashOperationalCostRepository
{
    Task<WashOperationalCost?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<WashOperationalCost?> GetByBranchAndMonthAsync(long branchId, DateTime monthYear, CancellationToken ct = default);
    Task<List<WashOperationalCost>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(WashOperationalCost entity, CancellationToken ct = default);
    Task UpdateAsync(WashOperationalCost entity, CancellationToken ct = default);
}
