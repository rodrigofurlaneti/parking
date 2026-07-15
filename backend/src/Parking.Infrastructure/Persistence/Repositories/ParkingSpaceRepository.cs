namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class ParkingSpaceRepository(AppDbContext context) : IParkingSpaceRepository
{
    public async Task<ParkingSpace?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.ParkingSpaces.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<ParkingSpace?> GetByNumberAsync(long branchId, string spaceNumber, CancellationToken ct = default)
    {
        return await context.ParkingSpaces.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BranchId == branchId && x.SpaceNumber == spaceNumber && x.IsActive, ct);
    }

    public async Task<List<ParkingSpace>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.ParkingSpaces.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<int> GetOccupiedCountAsync(long branchId, CancellationToken ct = default)
    {
        return await context.ParkingSpaces.AsNoTracking()
            .CountAsync(x => x.BranchId == branchId && x.Status == 1 && x.IsActive, ct);
    }

    public async Task AddAsync(ParkingSpace entity, CancellationToken ct = default)
    {
        await context.ParkingSpaces.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(ParkingSpace entity, CancellationToken ct = default)
    {
        context.ParkingSpaces.Update(entity);
        await Task.CompletedTask;
    }
}
