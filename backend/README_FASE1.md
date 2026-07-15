# Parking System — Fase 1: Foundation

## 🎯 Objetivo

Setup seguro e base organizacional. **MVP depends entirely on this.**

---

## 📋 Checklist de Início

### 1. Setup do Projeto .NET

```bash
cd backend

# Criar solução
dotnet new sln -n Parking

# Criar projects
dotnet new classlib -n Parking.Domain
dotnet new classlib -n Parking.Application
dotnet new classlib -n Parking.Infrastructure
dotnet new webapi -n Parking.API
dotnet new xunit -n Parking.Tests
dotnet new classlib -n Parking.Specs
dotnet new classlib -n Parking.ArchTests

# Adicionar à solução
dotnet sln add Parking.Domain/Parking.Domain.csproj
dotnet sln add Parking.Application/Parking.Application.csproj
dotnet sln add Parking.Infrastructure/Parking.Infrastructure.csproj
dotnet sln add Parking.API/Parking.API.csproj
dotnet sln add Parking.Tests/Parking.Tests.csproj
dotnet sln add Parking.Specs/Parking.Specs.csproj
dotnet sln add Parking.ArchTests/Parking.ArchTests.csproj

# Criar referências (dependências)
# Domain → nada
# Application → Domain
# Infrastructure → Domain
# API → Application + Infrastructure
# Tests → Application + Infrastructure
# Specs → Application
# ArchTests → todos
```

### 2. Instalar NuGets

Ver `Project_Structure.md` para lista completa.

### 3. Implementar Domain Layer

- [ ] `Parking.Domain/Common/Entity.cs`
- [ ] `Parking.Domain/Common/AggregateRoot.cs`
- [ ] `Parking.Domain/Common/ValueObject.cs`
- [ ] `Parking.Domain/Common/Error.cs`
- [ ] `Parking.Domain/Common/Result.cs`
- [ ] `Parking.Domain/Common/IDomainEvent.cs`

**Base classes template:** Ver `Parking.Domain_BaseClasses.cs`

### 4. Implementar Domain Entities

**Company (AggregateRoot):**
```csharp
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
            return Result.Failure<Company>(new Error("Company.InvalidName", "Name required."));
        
        return Result.Success(new Company(0, name.Trim(), cnpj.Trim(), legalName.Trim()));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Similarly for:** Branch, Role, Permission, AppUser, RefreshToken, UserRole, RolePermission, AccessLog

### 5. Implementar Domain Value Objects

**Email (ValueObject):**
```csharp
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
```

**Similarly for:** Username, PhoneNumber

### 6. Implementar Repository Interfaces

```csharp
public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task AddAsync(Company entity, CancellationToken ct = default);
    Task UpdateAsync(Company entity, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
```

**Similarly for:** IBranchRepository, IAppUserRepository, IRoleRepository, IPermissionRepository

### 7. Implementar Infrastructure

**AppDbContext:**
```csharp
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
        => modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

    public async Task<int> CommitAsync(CancellationToken ct = default)
        => await SaveChangesAsync(ct);
}
```

**EF Core Configurations:** (IEntityTypeConfiguration<T> implementations)
- CompanyConfiguration
- BranchConfiguration
- AppUserConfiguration (with owned Email, Username)
- RoleConfiguration
- RefreshTokenConfiguration
- UserRoleConfiguration
- RolePermissionConfiguration
- AccessLogConfiguration

**Repositories:**
- CompanyRepository : ICompanyRepository
- BranchRepository : IBranchRepository
- AppUserRepository : IAppUserRepository (with include for roles/permissions)
- RoleRepository : IRoleRepository
- PermissionRepository : IPermissionRepository

**Services:**
- PasswordHasher : IPasswordHasher (BCrypt)
- JwtTokenService : ITokenService
- CurrentUserService : ICurrentUser

### 8. Implementar Application Layer

**Messaging abstractions:**
```csharp
public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand { }
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse> { }
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }
```

**Features:**
- CreateUserCommand + Handler + Validator
- LoginCommand + Handler + Validator
- RefreshTokenCommand + Handler + Validator
- AssignRoleCommand + Handler + Validator
- GetAllUsersQuery + Handler
- GetAllRolesQuery + Handler
- CreateCompanyCommand + Handler + Validator
- CreateBranchCommand + Handler + Validator
- GetCompanyByIdQuery + Handler
- GetBranchesByCompanyQuery + Handler

**Pipeline Behaviors:**
- LoggingBehavior
- ValidationBehavior

**DTOs:**
- UserDto
- RoleDto
- CompanyDto
- BranchDto

### 9. Implementar API Layer

**Controllers:**
- ApiController (base with HandleFailure)
- AuthController (POST /register, /login, /refresh-token, /assign-role)
- CompanyController (POST, GET)
- BranchController (POST, GET)

**Middleware:**
- ExceptionHandlingMiddleware

**Program.cs:**
```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(/* config from appsettings */);

// ... use middleware ...
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

### 10. Implementar Testes

**Unit Tests (xUnit + NSubstitute):**
- CreateUserCommandHandlerTests
- LoginCommandHandlerTests
- RefreshTokenCommandHandlerTests
- CreateCompanyCommandHandlerTests

**BDD Tests (Reqnroll):**
- Authentication.feature + AuthenticationSteps.cs
- Company.feature + CompanySteps.cs

**Architecture Tests (NetArchTest):**
- Domain doesn't reference MediatR, EF Core, FluentValidation
- Application doesn't reference Infrastructure, API
- Handlers are `internal sealed`
- Repositories are `internal sealed`
- Responses are `sealed record`
- Validators inherit `AbstractValidator<>`

---

## 🗄️ Database Setup

```bash
# 1. Criar banco
sqlcmd -S YOUR_SERVER -i ../sql/Parking_DDL.sql

# 2. Seed dados
sqlcmd -S YOUR_SERVER -i ../sql/Parking_Seed.sql

# 3. EF Core migrations (post-implementation)
dotnet ef migrations add Initial --project Parking.Infrastructure --startup-project Parking.API
dotnet ef database update --project Parking.Infrastructure --startup-project Parking.API
```

---

## 🚀 Build & Run

```bash
# Build
dotnet build

# Run API
dotnet run --project Parking.API

# Run Tests
dotnet test

# Run specific test
dotnet test Parking.Tests --filter "CreateUserCommandHandlerTests"
```

---

## 📊 Success Metrics

- [x] DDL script runs without errors
- [x] Seed data populates roles, permissions, example users
- [ ] Solução .NET 9 criada com 7 projects
- [ ] Domain layer implementado (zero external dependencies)
- [ ] AppDbContext + EF Configurations rodando
- [ ] Repositories implementados
- [ ] Auth endpoints (register, login, refresh) funcionando
- [ ] JWT tokens gerados e validados
- [ ] Todos os tests passando (unit + arch)
- [ ] Clean Architecture patterns mantidos

---

## 📚 Referências

- Parking_Modelagem.md — Business modeling
- Parking_DDL.sql — Database schema
- Parking_Seed.sql — Initial data
- Fase1_Foundation.md — Full feature checklist
- Project_Structure.md — Project organization

---

## ⏱️ Timeline

- **Sprint 1:** Domain + Infrastructure (Week 1-2)
  - Base classes, entities, repositories, EF Core setup
  
- **Sprint 2:** Application + API (Week 3-4)
  - Commands, queries, handlers, controllers, tests
  
- **End of Phase 1:** MVP ready for Fase 2
  - All tests passing
  - Clean Architecture validated
  - Auth fully working

---

**Start Date:** 2026-07-15  
**Status:** 🟢 Ready to Code

Next Phase: 🟠 Fase 2 — Operacional (Employees + Cash + Parking Spaces)
