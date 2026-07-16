namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class MonthlyCustomerContractRepository(AppDbContext context) : IMonthlyCustomerContractRepository
{
    public async Task<List<MonthlyCustomerContract>> GetActiveByCustomerAsync(long customerId, CancellationToken ct = default)
    {
        return await context.MonthlyCustomerContracts.AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(MonthlyCustomerContract entity, CancellationToken ct = default)
    {
        await context.MonthlyCustomerContracts.AddAsync(entity, ct);
    }
}
