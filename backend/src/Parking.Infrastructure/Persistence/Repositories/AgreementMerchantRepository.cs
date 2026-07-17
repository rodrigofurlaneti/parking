namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class AgreementMerchantRepository(AppDbContext context) : IAgreementMerchantRepository
{
    public async Task<AgreementMerchant?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.AgreementMerchants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<AgreementMerchant>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.AgreementMerchants.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(AgreementMerchant entity, CancellationToken ct = default)
    {
        await context.AgreementMerchants.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(AgreementMerchant entity, CancellationToken ct = default)
    {
        context.AgreementMerchants.Update(entity);
        await Task.CompletedTask;
    }
}
