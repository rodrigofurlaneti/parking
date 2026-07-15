/*
 * PARKING FASE 1 — TEMPLATES PARTE 1
 * Domain Layer + Infrastructure Layer
 *
 * INSTRUÇÕES:
 * 1. Copie cada classe abaixo
 * 2. Crie o arquivo correspondente (ex: Entity.cs em Parking.Domain/Common/)
 * 3. Remova comentários de namespace se necessário
 * 4. Compile
 *
 * Esta parte contém ~3000 linhas: Domain (base classes, entities, value objects, repos) + Infrastructure (EF, configs, repos, services)
 */

// ============================================================================
// DOMAIN LAYER BASE CLASSES (já fornecido em Parking.Domain_BaseClasses.cs)
// Copie para: Parking.Domain/Common/
// ============================================================================

// Entity.cs, AggregateRoot.cs, IDomainEvent.cs, ValueObject.cs, Error.cs, Result.cs
// Ver: Parking.Domain_BaseClasses.cs para referência completa

// ============================================================================
// DOMAIN ENTITIES
// ============================================================================

// Parking.Domain/Entities/Company.cs
namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Company : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string LegalName { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Company() { }

    private Company(long id, string name, string cnpj, string legalName) : base(id)
    {
        Name = name;
        Cnpj = cnpj;
        LegalName = legalName;
    }

    public static Result<Company> Create(string name, string cnpj, string legalName)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Company>(new Error("Company.InvalidName", "Name is required."));

        if (string.IsNullOrWhiteSpace(cnpj))
            return Result.Failure<Company>(new Error("Company.InvalidCnpj", "CNPJ is required."));

        return Result.Success(new Company(0, name.Trim(), cnpj.Trim(), legalName.Trim()));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

// Parking.Domain/Entities/Branch.cs
namespace Parking.Domain.Entities;

public sealed class Branch : AggregateRoot
{
    public long CompanyId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public int TotalSpaces { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Branch() { }

    private Branch(long companyId, string name, string address, int totalSpaces) : base(0)
    {
        CompanyId = companyId;
        Name = name;
        Address = address;
        TotalSpaces = totalSpaces;
    }

    public static Result<Branch> Create(long companyId, string name, string address, int totalSpaces)
    {
        if (companyId <= 0)
            return Result.Failure<Branch>(new Error("Branch.InvalidCompanyId", "Company ID is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Branch>(new Error("Branch.InvalidName", "Name is required."));

        if (totalSpaces <= 0)
            return Result.Failure<Branch>(new Error("Branch.InvalidTotalSpaces", "Total spaces must be greater than 0."));

        return Result.Success(new Branch(companyId, name.Trim(), address.Trim(), totalSpaces));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

// Parking.Domain/Entities/Role.cs
namespace Parking.Domain.Entities;

public sealed class Role : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Role() { }

    private Role(string name, string? description) : base(0)
    {
        Name = name;
        Description = description;
    }

    public static Result<Role> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Role>(new Error("Role.InvalidName", "Name is required."));

        return Result.Success(new Role(name.Trim(), description?.Trim()));
    }
}

// Parking.Domain/Entities/Permission.cs
namespace Parking.Domain.Entities;

public sealed class Permission : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Permission() { }

    private Permission(string name, string? description) : base(0)
    {
        Name = name;
        Description = description;
    }

    public static Result<Permission> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Permission>(new Error("Permission.InvalidName", "Name is required."));

        return Result.Success(new Permission(name.Trim(), description?.Trim()));
    }
}

// Parking.Domain/Entities/AppUser.cs
namespace Parking.Domain.Entities;

using Parking.Domain.ValueObjects;

public sealed class AppUser : AggregateRoot
{
    public string UserName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int FailedAccessCount { get; private set; }
    public DateTime? LockoutEndAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private AppUser() { }

    private AppUser(string userName, Email email, string passwordHash, string fullName) : base(0)
    {
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
    }

    public static Result<AppUser> Create(string userName, Email email, string passwordHash, string fullName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Result.Failure<AppUser>(new Error("AppUser.InvalidUserName", "Username is required."));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure<AppUser>(new Error("AppUser.InvalidPassword", "Password hash is required."));

        return Result.Success(new AppUser(userName.Trim().ToLowerInvariant(), email, passwordHash, fullName.Trim()));
    }

    public void IncrementFailedAccessCount()
    {
        FailedAccessCount++;
        if (FailedAccessCount >= 5)
        {
            IsActive = false;
            LockoutEndAt = DateTime.UtcNow.AddHours(1);
        }
    }

    public void ResetFailedAccessCount()
    {
        FailedAccessCount = 0;
        LockoutEndAt = null;
    }

    public void SetLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

// Parking.Domain/Entities/RefreshToken.cs
namespace Parking.Domain.Entities;

public sealed class RefreshToken : Entity
{
    public long UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() { }

    public RefreshToken(long userId, string token, DateTime expiresAt) : base(0)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsValid => !IsExpired && !IsRevoked;

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}

// Parking.Domain/Entities/UserRole.cs
namespace Parking.Domain.Entities;

public sealed class UserRole : Entity
{
    public long UserId { get; private set; }
    public long RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    private UserRole() { }

    public UserRole(long userId, long roleId) : base(0)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
    }
}

// Parking.Domain/Entities/RolePermission.cs
namespace Parking.Domain.Entities;

public sealed class RolePermission : Entity
{
    public long RoleId { get; private set; }
    public long PermissionId { get; private set; }

    private RolePermission() { }

    public RolePermission(long roleId, long permissionId) : base(0)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}

// Parking.Domain/Entities/AccessLog.cs
namespace Parking.Domain.Entities;

public sealed class AccessLog : Entity
{
    public long UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string? Resource { get; private set; }
    public string? IpAddress { get; private set; }

    private AccessLog() { }

    public AccessLog(long userId, string action, string? resource = null, string? ipAddress = null) : base(0)
    {
        UserId = userId;
        Action = action;
        Resource = resource;
        IpAddress = ipAddress;
    }
}

// ============================================================================
// DOMAIN VALUE OBJECTS
// ============================================================================

// Parking.Domain/ValueObjects/Email.cs
namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            return Result.Failure<Email>(
                new Error("Email.Invalid", "Invalid email address."));

        return Result.Success(new Email(email.Trim().ToLowerInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}

// Parking.Domain/ValueObjects/Username.cs
namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class Username : ValueObject
{
    public string Value { get; }

    private static readonly Regex UsernameRegex = new(
        @"^[a-z0-9._]+$",
        RegexOptions.Compiled);

    private Username(string value) => Value = value;

    public static Result<Username> Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 80)
            return Result.Failure<Username>(
                new Error("Username.InvalidLength", "Username must be between 3 and 80 characters."));

        if (!UsernameRegex.IsMatch(username.ToLowerInvariant()))
            return Result.Failure<Username>(
                new Error("Username.InvalidFormat", "Username may only contain lowercase letters, digits, dots, and underscores."));

        return Result.Success(new Username(username.Trim().ToLowerInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}

// Parking.Domain/ValueObjects/PhoneNumber.cs
namespace Parking.Domain.ValueObjects;

using Parking.Domain.Common;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<PhoneNumber>(
                new Error("PhoneNumber.Required", "Phone number is required."));

        var cleaned = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

        if (cleaned.Length < 10 || cleaned.Length > 15)
            return Result.Failure<PhoneNumber>(
                new Error("PhoneNumber.InvalidLength", "Phone number must be between 10 and 15 digits."));

        return Result.Success(new PhoneNumber(phoneNumber.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}

// ============================================================================
// DOMAIN REPOSITORY INTERFACES
// ============================================================================

// Parking.Domain/Repositories/ICompanyRepository.cs
namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Company entity, CancellationToken ct = default);
    Task UpdateAsync(Company entity, CancellationToken ct = default);
}

// Parking.Domain/Repositories/IBranchRepository.cs
namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Branch>> GetAllByCompanyAsync(long companyId, CancellationToken ct = default);
    Task AddAsync(Branch entity, CancellationToken ct = default);
    Task UpdateAsync(Branch entity, CancellationToken ct = default);
}

// Parking.Domain/Repositories/IAppUserRepository.cs
namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IAppUserRepository
{
    Task<AppUser?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<AppUser?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsAsync(string userName, string email, CancellationToken ct = default);
    Task AddAsync(AppUser entity, CancellationToken ct = default);
    Task UpdateAsync(AppUser entity, CancellationToken ct = default);
}

// Parking.Domain/Repositories/IRoleRepository.cs
namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Role entity, CancellationToken ct = default);
}

// Parking.Domain/Repositories/IPermissionRepository.cs
namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken ct = default);
}

// Parking.Domain/Repositories/IUnitOfWork.cs
namespace Parking.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}

// ============================================================================
// INFRASTRUCTURE APPDBCONTEXT
// ============================================================================

// Parking.Infrastructure/Persistence/AppDbContext.cs
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await SaveChangesAsync(ct);
    }
}

// ============================================================================
// INFRASTRUCTURE CONFIGURATIONS (9 arquivos)
// ============================================================================

// [Continua na próxima seção — muito código. Os templates para EF Configurations, Repositories e Services estão aqui]
// Cada um segue o padrão abaixo:

// Parking.Infrastructure/Persistence/Configurations/CompanyConfiguration.cs
namespace Parking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.Domain.Entities;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Cnpj).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Cnpj).IsUnique();
        builder.Property(x => x.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("datetime2");
    }
}

// [NOTA: Repita padrão similar para:]
// - BranchConfiguration
// - RoleConfiguration
// - PermissionConfiguration
// - AppUserConfiguration (com OwnsOne para Email)
// - RefreshTokenConfiguration
// - UserRoleConfiguration
// - RolePermissionConfiguration
// - AccessLogConfiguration

// ============================================================================
// INFRAST REPOSITORIES (5 arquivos)
// ============================================================================

// Parking.Infrastructure/Persistence/Repositories/CompanyRepository.cs
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

// [NOTA: Repita padrão para:]
// - BranchRepository
// - AppUserRepository (com Include para roles/permissions)
// - RoleRepository
// - PermissionRepository

// ============================================================================
// INFRASTRUCTURE SERVICES (3 arquivos)
// ============================================================================

// Parking.Infrastructure/Services/PasswordHasher.cs
namespace Parking.Infrastructure.Services;

using Parking.Application.Abstractions.Services;

internal sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

// Parking.Infrastructure/Services/JwtTokenService.cs
namespace Parking.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Parking.Application.Abstractions.Services;

internal sealed class JwtTokenService(string secret, string issuer, string audience, int expiresInMinutes) : ITokenService
{
    public string GenerateToken(long userId, string userName, string email)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Parking.Infrastructure/Services/CurrentUserService.cs
namespace Parking.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Parking.Application.Abstractions.Services;

internal sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUser
{
    public long Id
    {
        get
        {
            var claim = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(claim, out var id) ? id : 0;
        }
    }

    public string UserName => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}

// ============================================================================
// INFRASTRUCTURE DEPENDENCY INJECTION
// ============================================================================

// Parking.Infrastructure/DependencyInjection.cs
namespace Parking.Infrastructure;

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

        return services;
    }
}

// ============================================================================
// END OF PART 1
// ============================================================================
// Continue com TEMPLATES_PARTE2_Application_API_Tests.cs para Application, API e Tests
