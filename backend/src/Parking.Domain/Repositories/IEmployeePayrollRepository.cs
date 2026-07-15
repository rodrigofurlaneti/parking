namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IEmployeePayrollRepository
{
    Task<EmployeePayroll?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<EmployeePayroll>> GetByEmployeeAsync(long employeeId, CancellationToken ct = default);
    Task AddAsync(EmployeePayroll entity, CancellationToken ct = default);
    Task UpdateAsync(EmployeePayroll entity, CancellationToken ct = default);
}
