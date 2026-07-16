namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IServiceCategoryRepository
{
    Task<ServiceCategory?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<ServiceCategory>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(ServiceCategory entity, CancellationToken ct = default);
    Task UpdateAsync(ServiceCategory entity, CancellationToken ct = default);
}
