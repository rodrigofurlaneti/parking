namespace Parking.Infrastructure;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

        // Repositories - Fase 4 (Faturamento)
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ISalePaymentRepository, SalePaymentRepository>();

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

        // Repositories - Fase Cliente (Cliente & Veiculo)
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVehicleModelRepository, VehicleModelRepository>();
        services.AddScoped<IAgreementMerchantRepository, AgreementMerchantRepository>();
        services.AddScoped<IAgreementCustomerContractRepository, AgreementCustomerContractRepository>();
        services.AddScoped<IMonthlyCustomerContractRepository, MonthlyCustomerContractRepository>();
        services.AddScoped<ITariffRepository, TariffRepository>();

        // Repositories - Fase Estoque (Fornecedor & Compras)
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IPurchaseItemRepository, PurchaseItemRepository>();

        // Repositories - Relatorios (read-model cross-aggregate)
        services.AddScoped<IReportsRepository, ReportsRepository>();

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        var jwtSettings = configuration.GetSection("Jwt");
        var jwtSecret = jwtSettings["Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
        var jwtIssuer = jwtSettings["Issuer"] ?? "Parking";
        var jwtAudience = jwtSettings["Audience"] ?? "Parking.Client";

        services.AddScoped<ITokenService>(sp =>
            new JwtTokenService(
                jwtSecret,
                jwtIssuer,
                jwtAudience,
                int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60")));

        services.AddScoped<ICurrentUser>(sp =>
            new CurrentUserService(sp.GetRequiredService<IHttpContextAccessor>()));

        services.AddHttpContextAccessor();

        // Autenticacao JWT Bearer - valida o token emitido pelo JwtTokenService (mesma chave/issuer/audience).
        // Sem isso, [Authorize] nos controllers nunca teria um handler de autenticacao configurado.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }
}
