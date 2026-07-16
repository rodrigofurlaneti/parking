namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class AgreementCustomerContractRepository(AppDbContext context) : IAgreementCustomerContractRepository
{
    public async Task<List<AgreementCustomerContract>> GetActiveByCustomerAsync(long customerId, CancellationToken ct = default)
    {
        return await context.AgreementCustomerContracts.AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(AgreementCustomerContract entity, CancellationToken ct = default)
    {
        await context.AgreementCustomerContracts.AddAsync(entity, ct);
    }
}
