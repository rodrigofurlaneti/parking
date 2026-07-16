namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IWashServiceRevenueRepository
{
    Task<WashServiceRevenue?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<WashServiceRevenue>> GetAllByScheduleAsync(long washScheduleId, CancellationToken ct = default);
    Task<List<WashServiceRevenue>> GetAllByPeriodAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task AddAsync(WashServiceRevenue entity, CancellationToken ct = default);
    Task UpdateAsync(WashServiceRevenue entity, CancellationToken ct = default);
}
