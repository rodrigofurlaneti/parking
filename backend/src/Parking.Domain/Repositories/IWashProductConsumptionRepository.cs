namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IWashProductConsumptionRepository
{
    Task<WashProductConsumption?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<WashProductConsumption>> GetAllByScheduleAsync(long washScheduleId, CancellationToken ct = default);
    Task AddAsync(WashProductConsumption entity, CancellationToken ct = default);
    Task UpdateAsync(WashProductConsumption entity, CancellationToken ct = default);
}
