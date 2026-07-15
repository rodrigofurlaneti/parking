namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class VehicleEntryRepository(AppDbContext context) : IVehicleEntryRepository
{
    public async Task<VehicleEntry?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.VehicleEntries.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<VehicleEntry?> GetByParkingSpaceAsync(long parkingSpaceId, CancellationToken ct = default)
    {
        return await context.VehicleEntries.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ParkingSpaceId == parkingSpaceId && x.Status == 0 && x.IsActive, ct);
    }

    public async Task<List<VehicleEntry>> GetParkedByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.VehicleEntries.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.Status == 0 && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(VehicleEntry entity, CancellationToken ct = default)
    {
        await context.VehicleEntries.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(VehicleEntry entity, CancellationToken ct = default)
    {
        context.VehicleEntries.Update(entity);
        await Task.CompletedTask;
    }
}
