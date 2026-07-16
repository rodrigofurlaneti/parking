namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class VehicleRepository(AppDbContext context) : IVehicleRepository
{
    public async Task<Vehicle?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Vehicles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken ct = default)
    {
        return await context.Vehicles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, ct);
    }

    public async Task<List<Vehicle>> GetByCustomerAsync(long customerId, CancellationToken ct = default)
    {
        return await context.Vehicles.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(ct);
    }

    public async Task<int> CountActiveByCustomerAsync(long customerId, CancellationToken ct = default)
    {
        return await context.Vehicles.AsNoTracking()
            .CountAsync(x => x.CustomerId == customerId && x.IsActive, ct);
    }

    public async Task AddAsync(Vehicle entity, CancellationToken ct = default)
    {
        await context.Vehicles.AddAsync(entity, ct);
    }
}
