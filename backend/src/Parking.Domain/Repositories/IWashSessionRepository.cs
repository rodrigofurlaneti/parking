namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IWashSessionRepository
{
    Task<WashSession?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<WashSession>> GetAllByScheduleAsync(long washScheduleId, CancellationToken ct = default);
    Task AddAsync(WashSession entity, CancellationToken ct = default);
    Task UpdateAsync(WashSession entity, CancellationToken ct = default);
}
