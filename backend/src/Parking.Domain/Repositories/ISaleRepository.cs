namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<long> GetNextSaleNumberAsync(long branchId, CancellationToken ct = default);
    Task<bool> ExistsActiveByVehicleExitAsync(long vehicleExitId, CancellationToken ct = default);
    Task AddAsync(Sale entity, CancellationToken ct = default);
}
