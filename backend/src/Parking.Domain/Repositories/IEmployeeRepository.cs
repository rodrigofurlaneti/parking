namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Employee?> GetByCPFAsync(string cpf, CancellationToken ct = default);
    Task<List<Employee>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(Employee entity, CancellationToken ct = default);
    Task UpdateAsync(Employee entity, CancellationToken ct = default);
}
