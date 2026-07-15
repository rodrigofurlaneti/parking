namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class VehicleExitRepository(AppDbContext context) : IVehicleExitRepository
{
    public async Task<VehicleExit?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.VehicleExits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<VehicleExit>> GetByEntryAsync(long vehicleEntryId, CancellationToken ct = default)
    {
        return await context.VehicleExits.AsNoTracking()
            .Where(x => x.VehicleEntryId == vehicleEntryId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(VehicleExit entity, CancellationToken ct = default)
    {
        await context.VehicleExits.AddAsync(entity, ct);
    }
}
