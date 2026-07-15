namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken ct = default);
}
