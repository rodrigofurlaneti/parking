namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class VehicleModelRepository(AppDbContext context) : IVehicleModelRepository
{
    public async Task<VehicleModel?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.VehicleModels.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<VehicleModel?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await context.VehicleModels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), ct);
    }

    public async Task<List<VehicleModel>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.VehicleModels.AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<List<VehicleModel>> SearchAsync(string prefix, int limit, CancellationToken ct = default)
    {
        return await context.VehicleModels.AsNoTracking()
            .Where(x => x.IsActive && x.Name.ToLower().StartsWith(prefix.ToLower()))
            .OrderBy(x => x.Name)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task AddAsync(VehicleModel entity, CancellationToken ct = default)
    {
        await context.VehicleModels.AddAsync(entity, ct);
    }
}
