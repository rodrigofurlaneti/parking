namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    public async Task<Employee?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Employee?> GetByCPFAsync(string cpf, CancellationToken ct = default)
    {
        return await context.Employees.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CPF == cpf && x.IsActive, ct);
    }

    public async Task<List<Employee>> GetAllByBranchAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Employees.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Employee entity, CancellationToken ct = default)
    {
        await context.Employees.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Employee entity, CancellationToken ct = default)
    {
        context.Employees.Update(entity);
        await Task.CompletedTask;
    }
}
