namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class SaleRepository(AppDbContext context) : ISaleRepository
{
    public async Task<Sale?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<long> GetNextSaleNumberAsync(long branchId, CancellationToken ct = default)
    {
        var lastSaleNumber = await context.Sales.AsNoTracking()
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.SaleNumber)
            .Select(x => x.SaleNumber)
            .FirstOrDefaultAsync(ct);

        return lastSaleNumber + 1;
    }

    public async Task<bool> ExistsActiveByVehicleExitAsync(long vehicleExitId, CancellationToken ct = default)
    {
        return await context.Sales.AsNoTracking()
            .AnyAsync(x => x.VehicleExitId == vehicleExitId && x.IsActive, ct);
    }

    public async Task AddAsync(Sale entity, CancellationToken ct = default)
    {
        await context.Sales.AddAsync(entity, ct);
    }
}
