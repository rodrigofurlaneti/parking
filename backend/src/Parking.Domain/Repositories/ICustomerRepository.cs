namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Customer?> GetByDocumentAsync(string document, CancellationToken ct = default);
    Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct = default);
    Task AddAsync(Customer entity, CancellationToken ct = default);
    Task UpdateAsync(Customer entity, CancellationToken ct = default);
}
