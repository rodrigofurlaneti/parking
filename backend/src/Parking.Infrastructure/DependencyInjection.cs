namespace Parking.Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parking.Application.Abstractions.Services;
using Parking.Domain.Repositories;
using Parking.Infrastructure.Persistence;
using Parking.Infrastructure.Persistence.Repositories;
using Parking.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // UnitOfWork
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        // Repositories - Fase 2 (Operacional)
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeScheduleRepository, EmployeeScheduleRepository>();
        services.AddScoped<IEmployeePayrollRepository, EmployeePayrollRepository>();
        services.AddScoped<ICashRegisterRepository, CashRegisterRepository>();
        services.AddScoped<ICashMovementRepository, CashMovementRepository>();
        services.AddScoped<IParkingSpaceRepository, ParkingSpaceRepository>();
        services.AddScoped<IVehicleEntryRepository, VehicleEntryRepository>();
        services.AddScoped<IVehicleExitRepository, VehicleExitRepository>();

        // Repositories - Fase 3 (Lava Rapido)
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<IServiceItemRepository, ServiceItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWashScheduleRepository, WashScheduleRepository>();
        services.AddScoped<IWashSessionRepository, WashSessionRepository>();
        services.AddScoped<IWashEmployeeRepository, WashEmployeeRepository>();
        services.AddScoped<IWashOperationalCostRepository, WashOperationalCostRepository>();
        services.AddScoped<IWashServiceRevenueRepository, WashServiceRevenueRepository>();
        services.AddScoped<IWashProductConsumptionRepository, WashProductConsumptionRepository>();

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        var jwtSettings = configuration.GetSection("Jwt");
        services.AddScoped<ITokenService>(sp =>
            new JwtTokenService(
                jwtSettings["Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured"),
                jwtSettings["Issuer"] ?? "Parking",
                jwtSettings["Audience"] ?? "Parking.Client",
                int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60")));

        services.AddScoped<ICurrentUser>(sp =>
            new CurrentUserService(sp.GetRequiredService<IHttpContextAccessor>()));

        services.AddHttpContextAccessor();

        return services;
    }
}
