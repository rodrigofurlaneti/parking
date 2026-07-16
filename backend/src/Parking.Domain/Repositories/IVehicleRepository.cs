namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken ct = default);
    Task<List<Vehicle>> GetByCustomerAsync(long customerId, CancellationToken ct = default);
    Task<int> CountActiveByCustomerAsync(long customerId, CancellationToken ct = default);
    Task AddAsync(Vehicle entity, CancellationToken ct = default);
}
