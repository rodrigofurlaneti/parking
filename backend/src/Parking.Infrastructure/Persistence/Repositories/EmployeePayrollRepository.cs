namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class EmployeePayrollRepository(AppDbContext context) : IEmployeePayrollRepository
{
    public async Task<EmployeePayroll?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.EmployeePayrolls.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<EmployeePayroll>> GetByEmployeeAsync(long employeeId, CancellationToken ct = default)
    {
        return await context.EmployeePayrolls.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(EmployeePayroll entity, CancellationToken ct = default)
    {
        await context.EmployeePayrolls.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(EmployeePayroll entity, CancellationToken ct = default)
    {
        context.EmployeePayrolls.Update(entity);
        await Task.CompletedTask;
    }
}
