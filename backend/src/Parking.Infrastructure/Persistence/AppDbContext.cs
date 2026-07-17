namespace Parking.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeSchedule> EmployeeSchedules => Set<EmployeeSchedule>();
    public DbSet<EmployeePayroll> EmployeePayrolls => Set<EmployeePayroll>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();
    public DbSet<ParkingSpace> ParkingSpaces => Set<ParkingSpace>();
    public DbSet<VehicleEntry> VehicleEntries => Set<VehicleEntry>();
    public DbSet<VehicleExit> VehicleExits => Set<VehicleExit>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SalePayment> SalePayments => Set<SalePayment>();

    // Fase 3 - Lava Rapido
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<WashSchedule> WashSchedules => Set<WashSchedule>();
    public DbSet<WashSession> WashSessions => Set<WashSession>();
    public DbSet<WashEmployee> WashEmployees => Set<WashEmployee>();
    public DbSet<WashOperationalCost> WashOperationalCosts => Set<WashOperationalCost>();
    public DbSet<WashServiceRevenue> WashServiceRevenues => Set<WashServiceRevenue>();
    public DbSet<WashProductConsumption> WashProductConsumptions => Set<WashProductConsumption>();

    // Fase Cliente - Cliente & Veiculo
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<AgreementMerchant> AgreementMerchants => Set<AgreementMerchant>();
    public DbSet<AgreementCustomerContract> AgreementCustomerContracts => Set<AgreementCustomerContract>();
    public DbSet<MonthlyCustomerContract> MonthlyCustomerContracts => Set<MonthlyCustomerContract>();
    public DbSet<Tariff> Tariffs => Set<Tariff>();

    // Fase Estoque - Fornecedor & Compras
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _currentTransaction = await Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
            return;

        try
        {
            await _currentTransaction.CommitAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
            return;

        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
