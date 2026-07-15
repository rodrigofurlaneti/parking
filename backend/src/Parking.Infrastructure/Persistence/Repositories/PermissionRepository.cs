namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class PermissionRepository(AppDbContext context) : IPermissionRepository
{
    public async Task<Permission?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Permissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Permissions.AsNoTracking().ToListAsync(ct);
    }
}
