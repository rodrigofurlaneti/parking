namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IAppUserRepository
{
    Task<AppUser?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<AppUser?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(string userName, string email, CancellationToken ct = default);
    Task AddAsync(AppUser entity, CancellationToken ct = default);
    Task UpdateAsync(AppUser entity, CancellationToken ct = default);
}
