namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Company entity, CancellationToken ct = default);
    Task UpdateAsync(Company entity, CancellationToken ct = default);
}
