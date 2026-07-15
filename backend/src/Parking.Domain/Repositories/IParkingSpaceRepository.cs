namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IParkingSpaceRepository
{
    Task<ParkingSpace?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ParkingSpace?> GetByNumberAsync(long branchId, string spaceNumber, CancellationToken ct = default);
    Task<List<ParkingSpace>> GetAllByBranchAsync(long branchId, CancellationToken ct = default);
    Task<int> GetOccupiedCountAsync(long branchId, CancellationToken ct = default);
    Task AddAsync(ParkingSpace entity, CancellationToken ct = default);
    Task UpdateAsync(ParkingSpace entity, CancellationToken ct = default);
}
