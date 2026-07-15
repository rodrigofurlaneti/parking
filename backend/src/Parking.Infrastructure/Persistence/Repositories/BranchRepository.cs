namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class BranchRepository(AppDbContext context) : IBranchRepository
{
    public async Task<Branch?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Branch>> GetAllByCompanyAsync(long companyId, CancellationToken ct = default)
    {
        return await context.Branches.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Branch entity, CancellationToken ct = default)
    {
        await context.Branches.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Branch entity, CancellationToken ct = default)
    {
        context.Branches.Update(entity);
        await Task.CompletedTask;
    }
}
