namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IVehicleModelRepository
{
    Task<VehicleModel?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<VehicleModel?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<List<VehicleModel>> GetAllAsync(CancellationToken ct = default);
    // Prefix search usado pelo autocomplete do frontend: conforme o funcionario digita,
    // sugere os modelos mais proximos que ja existem no catalogo.
    Task<List<VehicleModel>> SearchAsync(string prefix, int limit, CancellationToken ct = default);
    Task AddAsync(VehicleModel entity, CancellationToken ct = default);
}
