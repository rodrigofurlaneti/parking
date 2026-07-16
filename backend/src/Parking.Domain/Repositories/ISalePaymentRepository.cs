namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ISalePaymentRepository
{
    Task<List<SalePayment>> GetBySaleAsync(long saleId, CancellationToken ct = default);
    Task AddAsync(SalePayment entity, CancellationToken ct = default);
}
