namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ICashRegisterRepository
{
    Task<CashRegister?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<CashRegister?> GetOpenByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(CashRegister entity, CancellationToken ct = default);
    Task UpdateAsync(CashRegister entity, CancellationToken ct = default);
}
