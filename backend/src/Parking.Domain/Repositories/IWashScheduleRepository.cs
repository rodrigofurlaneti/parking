namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IWashScheduleRepository
{
    Task<WashSchedule?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<WashSchedule>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task<List<WashSchedule>> GetAllByEmployeeAsync(long employeeId, CancellationToken ct = default);
    Task AddAsync(WashSchedule entity, CancellationToken ct = default);
    Task UpdateAsync(WashSchedule entity, CancellationToken ct = default);
}
