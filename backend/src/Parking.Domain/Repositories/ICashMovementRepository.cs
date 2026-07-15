namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ICashMovementRepository
{
    Task<CashMovement?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<CashMovement>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken ct = default);
    Task<decimal> GetTotalByRegisterAsync(long cashRegisterId, CancellationToken ct = default);
    Task AddAsync(CashMovement entity, CancellationToken ct = default);
}
