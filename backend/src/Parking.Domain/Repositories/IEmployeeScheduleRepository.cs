namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IEmployeeScheduleRepository
{
    Task<EmployeeSchedule?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<EmployeeSchedule>> GetByEmployeeAsync(long employeeId, CancellationToken ct = default);
    Task AddAsync(EmployeeSchedule entity, CancellationToken ct = default);
    Task UpdateAsync(EmployeeSchedule entity, CancellationToken ct = default);
}
