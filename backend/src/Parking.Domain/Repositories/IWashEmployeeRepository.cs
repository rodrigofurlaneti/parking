namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IWashEmployeeRepository
{
    Task<WashEmployee?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<WashEmployee?> GetByEmployeeIdAsync(long employeeId, CancellationToken ct = default);
    Task<List<WashEmployee>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(WashEmployee entity, CancellationToken ct = default);
    Task UpdateAsync(WashEmployee entity, CancellationToken ct = default);
}
