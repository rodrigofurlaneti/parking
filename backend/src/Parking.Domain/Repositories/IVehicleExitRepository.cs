namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IVehicleExitRepository
{
    Task<VehicleExit?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<VehicleExit>> GetByEntryAsync(long vehicleEntryId, CancellationToken ct = default);
    Task AddAsync(VehicleExit entity, CancellationToken ct = default);
}
