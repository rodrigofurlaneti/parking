namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class SalePaymentRepository(AppDbContext context) : ISalePaymentRepository
{
    public async Task<List<SalePayment>> GetBySaleAsync(long saleId, CancellationToken ct = default)
    {
        return await context.SalePayments.AsNoTracking()
            .Where(x => x.SaleId == saleId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(SalePayment entity, CancellationToken ct = default)
    {
        await context.SalePayments.AddAsync(entity, ct);
    }
}
