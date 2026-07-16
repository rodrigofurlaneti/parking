namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class ReportsRepository(AppDbContext context) : IReportsRepository
{
    public async Task<int> CountActiveParkingSpacesAsync(long branchId, CancellationToken ct = default)
    {
        return await context.ParkingSpaces.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .CountAsync(ct);
    }

    public async Task<List<VehicleEntry>> GetVehicleEntriesAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await context.VehicleEntries.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.EntryTime >= fromDate && x.EntryTime <= toDate)
            .ToListAsync(ct);
    }

    public async Task<List<VehicleExit>> GetVehicleExitsAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await (from exit in context.VehicleExits.AsNoTracking()
                       join entry in context.VehicleEntries.AsNoTracking() on exit.VehicleEntryId equals entry.Id
                       where entry.BranchId == branchId && exit.ExitTime >= fromDate && exit.ExitTime <= toDate
                       select exit).ToListAsync(ct);
    }

    public async Task<List<SaleWithCustomerType>> GetSalesWithCustomerTypeAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        var query = from sale in context.Sales.AsNoTracking()
                     join exit in context.VehicleExits.AsNoTracking() on sale.VehicleExitId equals exit.Id
                     join entry in context.VehicleEntries.AsNoTracking() on exit.VehicleEntryId equals entry.Id
                     join customer in context.Customers.AsNoTracking() on entry.CustomerId equals customer.Id
                     where sale.BranchId == branchId
                        && sale.SaleDate >= fromDate && sale.SaleDate <= toDate
                        && sale.IsActive
                     select new SaleWithCustomerType(sale, customer.CustomerType);

        return await query.ToListAsync(ct);
    }

    public async Task<decimal> GetWashServiceRevenueTotalAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        var query = from revenue in context.WashServiceRevenues.AsNoTracking()
                     join schedule in context.WashSchedules.AsNoTracking() on revenue.WashScheduleId equals schedule.Id
                     where schedule.BranchId == branchId
                        && revenue.Date >= fromDate && revenue.Date <= toDate
                        && revenue.IsActive
                     select revenue.TotalPrice;

        return await query.SumAsync(ct);
    }

    public async Task<List<Product>> GetActiveProductsAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Products.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<List<StockMovement>> GetRecentStockMovementsAsync(long branchId, DateTime sinceDate, CancellationToken ct = default)
    {
        return await (from movement in context.StockMovements.AsNoTracking()
                       join product in context.Products.AsNoTracking() on movement.ProductId equals product.Id
                       where product.BranchId == branchId && movement.MovementDate >= sinceDate
                       select movement).ToListAsync(ct);
    }

    public async Task<List<Employee>> GetActiveEmployeesAsync(long branchId, CancellationToken ct = default)
    {
        return await context.Employees.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<List<EmployeeSchedule>> GetEmployeeSchedulesAsync(IReadOnlyCollection<long> employeeIds, CancellationToken ct = default)
    {
        return await context.EmployeeSchedules.AsNoTracking()
            .Where(x => employeeIds.Contains(x.EmployeeId) && x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<List<EmployeeWashSessionCount>> GetCompletedWashSessionCountsByEmployeeAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        var query = from session in context.WashSessions.AsNoTracking()
                     join schedule in context.WashSchedules.AsNoTracking() on session.WashScheduleId equals schedule.Id
                     where schedule.BranchId == branchId
                        && session.Status == 1
                        && session.EndTime != null
                        && session.EndTime >= fromDate && session.EndTime <= toDate
                     group session by schedule.EmployeeId into g
                     select new EmployeeWashSessionCount(g.Key, g.Count());

        return await query.ToListAsync(ct);
    }

    public async Task<List<CashRegister>> GetClosedCashRegistersAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await context.CashRegisters.AsNoTracking()
            .Where(x => x.BranchId == branchId
                && x.Status == 1
                && x.ClosedAt != null
                && x.ClosedAt >= fromDate && x.ClosedAt <= toDate)
            .ToListAsync(ct);
    }

    public async Task<List<CashMovement>> GetCashMovementsAsync(IReadOnlyCollection<long> cashRegisterIds, CancellationToken ct = default)
    {
        return await context.CashMovements.AsNoTracking()
            .Where(x => cashRegisterIds.Contains(x.CashRegisterId) && x.IsActive)
            .ToListAsync(ct);
    }
}
