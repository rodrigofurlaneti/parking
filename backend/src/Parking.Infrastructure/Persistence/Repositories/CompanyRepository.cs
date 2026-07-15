namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CompanyRepository(AppDbContext context) : ICompanyRepository
{
    public async Task<Company?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        return await context.Companies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Cnpj == cnpj && x.IsActive, ct);
    }

    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Companies.AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Company entity, CancellationToken ct = default)
    {
        await context.Companies.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Company entity, CancellationToken ct = default)
    {
        context.Companies.Update(entity);
        await Task.CompletedTask;
    }
}
