namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CashRegisterRepository(AppDbContext context) : ICashRegisterRepository
{
    public async Task<CashRegister?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.CashRegisters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<CashRegister?> GetOpenByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.CashRegisters.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BranchId == branchId && x.Status == 0 && x.IsActive, ct);
    }

    public async Task AddAsync(CashRegister entity, CancellationToken ct = default)
    {
        await context.CashRegisters.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(CashRegister entity, CancellationToken ct = default)
    {
        context.CashRegisters.Update(entity);
        await Task.CompletedTask;
    }
}
