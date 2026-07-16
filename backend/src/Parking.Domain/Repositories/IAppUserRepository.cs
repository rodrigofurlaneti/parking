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

    /// <summary>
    /// Retorna os nomes das roles ativas atribuidas ao usuario (via tabela UserRole/Role).
    /// Usado para popular a claim de role no JWT emitido no login.
    /// </summary>
    Task<IReadOnlyList<string>> GetRoleNamesAsync(long userId, CancellationToken ct = default);

    /// <summary>
    /// Persiste o vinculo usuario-role (tabela UserRole). Idempotente: nao duplica se ja existir.
    /// </summary>
    Task AddRoleToUserAsync(long userId, long roleId, CancellationToken ct = default);
}
