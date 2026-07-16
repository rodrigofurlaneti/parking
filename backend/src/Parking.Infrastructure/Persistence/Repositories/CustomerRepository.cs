namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Customer?> GetByDocumentAsync(string document, CancellationToken ct = default)
    {
        return await context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Document == document, ct);
    }

    public async Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct = default)
    {
        return await context.Customers.AsNoTracking().AnyAsync(x => x.Document == document, ct);
    }

    public async Task AddAsync(Customer entity, CancellationToken ct = default)
    {
        await context.Customers.AddAsync(entity, ct);
    }

    public Task UpdateAsync(Customer entity, CancellationToken ct = default)
    {
        context.Customers.Update(entity);
        return Task.CompletedTask;
    }
}
