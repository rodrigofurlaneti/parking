namespace Parking.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork
{
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await SaveChangesAsync(ct);
    }
}
