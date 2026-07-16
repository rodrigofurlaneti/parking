namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IServiceItemRepository
{
    Task<ServiceItem?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<ServiceItem>> GetAllByCategoryAsync(long serviceCategoryId, CancellationToken ct = default);
    Task AddAsync(ServiceItem entity, CancellationToken ct = default);
    Task UpdateAsync(ServiceItem entity, CancellationToken ct = default);
}
