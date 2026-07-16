namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

/// <summary>
/// Dedicated read-model repository for cross-aggregate reporting queries.
/// Not tied to a single aggregate root; implementations may query AppDbContext directly.
/// </summary>
public interface IReportsRepository
{
    // Occupancy report
    Task<int> CountActiveParkingSpacesAsync(long branchId, CancellationToken ct = default);
    Task<List<VehicleEntry>> GetVehicleEntriesAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<List<VehicleExit>> GetVehicleExitsAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);

    // Revenue report
    Task<List<SaleWithCustomerType>> GetSalesWithCustomerTypeAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<decimal> GetWashServiceRevenueTotalAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);

    // Stock report
    Task<List<Product>> GetActiveProductsAsync(long branchId, CancellationToken ct = default);
    Task<List<StockMovement>> GetRecentStockMovementsAsync(long branchId, DateTime sinceDate, CancellationToken ct = default);

    // Employee report
    Task<List<Employee>> GetActiveEmployeesAsync(long branchId, CancellationToken ct = default);
    Task<List<EmployeeSchedule>> GetEmployeeSchedulesAsync(IReadOnlyCollection<long> employeeIds, CancellationToken ct = default);
    Task<List<EmployeeWashSessionCount>> GetCompletedWashSessionCountsByEmployeeAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);

    // Cash report
    Task<List<CashRegister>> GetClosedCashRegistersAsync(long branchId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<List<CashMovement>> GetCashMovementsAsync(IReadOnlyCollection<long> cashRegisterIds, CancellationToken ct = default);
}

public sealed record SaleWithCustomerType(Sale Sale, int CustomerType);

public sealed record EmployeeWashSessionCount(long EmployeeId, int Count);
