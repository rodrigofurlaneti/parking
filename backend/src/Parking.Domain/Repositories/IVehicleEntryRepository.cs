namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IVehicleEntryRepository
{
    Task<VehicleEntry?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<VehicleEntry?> GetByParkingSpaceAsync(long parkingSpaceId, CancellationToken ct = default);
    Task<List<VehicleEntry>> GetParkedByBranchAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(VehicleEntry entity, CancellationToken ct = default);
    Task UpdateAsync(VehicleEntry entity, CancellationToken ct = default);
}
