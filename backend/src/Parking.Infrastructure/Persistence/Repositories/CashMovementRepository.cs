namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CashMovementRepository(AppDbContext context) : ICashMovementRepository
{
    public async Task<CashMovement?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.CashMovements.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<CashMovement>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken ct = default)
    {
        return await context.CashMovements.AsNoTracking()
            .Where(x => x.CashRegisterId == cashRegisterId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalByRegisterAsync(long cashRegisterId, CancellationToken ct = default)
    {
        return await context.CashMovements.AsNoTracking()
            .Where(x => x.CashRegisterId == cashRegisterId && x.IsActive)
            .SumAsync(x => x.Amount, ct);
    }

    public async Task AddAsync(CashMovement entity, CancellationToken ct = default)
    {
        await context.CashMovements.AddAsync(entity, ct);
    }
}
