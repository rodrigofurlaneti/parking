namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Branch>> GetAllByCompanyAsync(long companyId, CancellationToken ct = default);
    Task AddAsync(Branch entity, CancellationToken ct = default);
    Task UpdateAsync(Branch entity, CancellationToken ct = default);
}
