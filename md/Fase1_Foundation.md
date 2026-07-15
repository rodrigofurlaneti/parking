# Fase 1 — Foundation: Autenticação & Organização

**Status:** 🔴 INICIANDO  
**Estimativa:** 2 sprints  
**Prioridade:** CRÍTICA  
**Data Início:** 2026-07-15

---

## Objetivo

Setup seguro e base organizacional para o sistema. Sem isso, nada funciona.

---

## Deliverables

### 1. Domain Layer
✅ **Entities:**
- [ ] `Company` (AggregateRoot)
- [ ] `Branch` (AggregateRoot)
- [ ] `Role` (AggregateRoot)
- [ ] `Permission` (AggregateRoot)
- [ ] `AppUser` (AggregateRoot) — com VerifyPassword()
- [ ] `RefreshToken` (Entity)
- [ ] `UserRole` (Entity, join table)
- [ ] `RolePermission` (Entity, join table)
- [ ] `AccessLog` (Entity)

✅ **Value Objects:**
- [ ] `Email`
- [ ] `Username`
- [ ] `PhoneNumber`

✅ **Repository Interfaces:**
- [ ] `ICompanyRepository`
- [ ] `IBranchRepository`
- [ ] `IAppUserRepository`
- [ ] `IRoleRepository`
- [ ] `IPermissionRepository`
- [ ] `IUnitOfWork`

### 2. Infrastructure Layer
✅ **EF Core:**
- [ ] `AppDbContext` (implements IUnitOfWork)
- [ ] `IEntityTypeConfiguration<Company>`
- [ ] `IEntityTypeConfiguration<Branch>`
- [ ] `IEntityTypeConfiguration<AppUser>` (with owned Email, Username)
- [ ] `IEntityTypeConfiguration<Role>`
- [ ] `IEntityTypeConfiguration<Permission>`
- [ ] `IEntityTypeConfiguration<RefreshToken>`
- [ ] `IEntityTypeConfiguration<UserRole>`
- [ ] `IEntityTypeConfiguration<RolePermission>`
- [ ] `IEntityTypeConfiguration<AccessLog>`

✅ **Repositories:**
- [ ] `CompanyRepository`
- [ ] `BranchRepository`
- [ ] `AppUserRepository`
- [ ] `RoleRepository`
- [ ] `PermissionRepository`

✅ **Services:**
- [ ] `PasswordHasher : IPasswordHasher` (BCrypt workFactor=12)
- [ ] `JwtTokenService : ITokenService`
- [ ] `CurrentUserService : ICurrentUser`

✅ **DI:**
- [ ] `AddInfrastructure(IServiceCollection, IConfiguration)`

### 3. Application Layer
✅ **Abstractions:**
- [ ] `ICommand`, `ICommand<T>`
- [ ] `ICommandHandler<T>`, `ICommandHandler<T, R>`
- [ ] `IQuery<T>`
- [ ] `IQueryHandler<T, R>`

✅ **Features: Auth**
- [ ] `CreateUserCommand` + Handler + Validator
- [ ] `LoginCommand` + Handler + Validator
- [ ] `RefreshTokenCommand` + Handler + Validator
- [ ] `AssignRoleCommand` + Handler + Validator
- [ ] `GetAllUsersQuery` + Handler
- [ ] `GetAllRolesQuery` + Handler

✅ **Features: Company & Branch**
- [ ] `CreateCompanyCommand` + Handler + Validator
- [ ] `CreateBranchCommand` + Handler + Validator
- [ ] `GetCompanyByIdQuery` + Handler
- [ ] `GetBranchesByCompanyQuery` + Handler

✅ **Pipeline Behaviors:**
- [ ] `LoggingBehavior<TRequest, TResponse>`
- [ ] `ValidationBehavior<TRequest, TResponse>`

### 4. API Layer
✅ **Middleware:**
- [ ] `ExceptionHandlingMiddleware`
- [ ] CORS setup
- [ ] Request/Response logging

✅ **Controllers:**
- [ ] `ApiController` (base class with HandleFailure)
- [ ] `AuthController`
- [ ] `CompanyController`
- [ ] `BranchController`

✅ **Program.cs:**
- [ ] `services.AddApplication()`
- [ ] `services.AddInfrastructure(configuration)`
- [ ] `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`
- [ ] Middleware registration

### 5. Database
- [x] `Parking_DDL.sql` (48 tabelas)
- [x] `Parking_Seed.sql` (roles, permissions, example data)

### 6. Tests
✅ **Architecture Tests (NetArchTest):**
- [ ] Domain não referencia MediatR, EF Core, FluentValidation
- [ ] Application não referencia Infrastructure, API
- [ ] All handlers `internal sealed`
- [ ] All repositories `internal sealed`
- [ ] All responses `sealed record`
- [ ] All validators herdam `AbstractValidator<>`

✅ **Unit Tests (xUnit + NSubstitute):**
- [ ] `CreateUserCommandHandlerTests`
  - [ ] Reject duplicate username
  - [ ] Reject duplicate email
  - [ ] Successfully create user
  - [ ] Password hashed before save

- [ ] `LoginCommandHandlerTests`
  - [ ] Reject invalid username
  - [ ] Reject invalid password
  - [ ] Reject locked user
  - [ ] Successfully login with tokens

- [ ] `RefreshTokenCommandHandlerTests`
  - [ ] Reject expired token
  - [ ] Reject revoked token
  - [ ] Successfully refresh

- [ ] `CreateCompanyCommandHandlerTests`
  - [ ] Reject duplicate CNPJ
  - [ ] Successfully create

✅ **BDD Tests (Reqnroll):**
- [ ] `Authentication.feature`
  - [ ] Register new user
  - [ ] Reject duplicate email
  - [ ] Login valid credentials
  - [ ] Login invalid password
  - [ ] Refresh token

- [ ] `Company.feature`
  - [ ] Create company
  - [ ] Reject duplicate CNPJ

---

## Project Structure

```
Parking/
└── backend/
    ├── Parking.Domain/
    │   ├── Entities/
    │   │   ├── Company.cs
    │   │   ├── Branch.cs
    │   │   ├── Role.cs
    │   │   ├── AppUser.cs
    │   │   ├── RefreshToken.cs
    │   │   └── AccessLog.cs
    │   ├── ValueObjects/
    │   │   ├── Email.cs
    │   │   ├── Username.cs
    │   │   └── PhoneNumber.cs
    │   ├── Repositories/
    │   │   ├── ICompanyRepository.cs
    │   │   ├── IBranchRepository.cs
    │   │   ├── IAppUserRepository.cs
    │   │   ├── IRoleRepository.cs
    │   │   ├── IPermissionRepository.cs
    │   │   └── IUnitOfWork.cs
    │   └── Common/
    │       ├── Entity.cs
    │       ├── AggregateRoot.cs
    │       ├── ValueObject.cs
    │       ├── Error.cs
    │       └── Result.cs
    │
    ├── Parking.Application/
    │   ├── Abstractions/
    │   │   ├── Messaging/
    │   │   │   ├── ICommand.cs
    │   │   │   ├── ICommandHandler.cs
    │   │   │   ├── IQuery.cs
    │   │   │   └── IQueryHandler.cs
    │   │   └── Services/
    │   │       ├── IPasswordHasher.cs
    │   │       ├── ITokenService.cs
    │   │       └── ICurrentUser.cs
    │   ├── Features/
    │   │   ├── Auth/
    │   │   │   ├── CreateUser/
    │   │   │   │   ├── CreateUserCommand.cs
    │   │   │   │   ├── CreateUserCommandHandler.cs
    │   │   │   │   └── CreateUserCommandValidator.cs
    │   │   │   ├── Login/
    │   │   │   ├── RefreshToken/
    │   │   │   ├── AssignRole/
    │   │   │   └── GetUsers/
    │   │   ├── Company/
    │   │   │   ├── Create/
    │   │   │   ├── GetById/
    │   │   │   └── GetAll/
    │   │   ├── Branch/
    │   │   │   ├── Create/
    │   │   │   └── GetById/
    │   │   └── Common/
    │   │       └── DTOs/
    │   │           ├── UserDto.cs
    │   │           ├── RoleDto.cs
    │   │           ├── CompanyDto.cs
    │   │           └── BranchDto.cs
    │   ├── Behaviors/
    │   │   ├── LoggingBehavior.cs
    │   │   └── ValidationBehavior.cs
    │   └── DependencyInjection.cs
    │
    ├── Parking.Infrastructure/
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs
    │   │   ├── Configurations/
    │   │   │   ├── CompanyConfiguration.cs
    │   │   │   ├── BranchConfiguration.cs
    │   │   │   ├── AppUserConfiguration.cs
    │   │   │   ├── RoleConfiguration.cs
    │   │   │   ├── RefreshTokenConfiguration.cs
    │   │   │   └── etc...
    │   │   └── Repositories/
    │   │       ├── CompanyRepository.cs
    │   │       ├── BranchRepository.cs
    │   │       ├── AppUserRepository.cs
    │   │       ├── RoleRepository.cs
    │   │       └── PermissionRepository.cs
    │   ├── Services/
    │   │   ├── PasswordHasher.cs
    │   │   ├── JwtTokenService.cs
    │   │   └── CurrentUserService.cs
    │   └── DependencyInjection.cs
    │
    ├── Parking.API/
    │   ├── Controllers/
    │   │   ├── ApiController.cs
    │   │   ├── AuthController.cs
    │   │   ├── CompanyController.cs
    │   │   └── BranchController.cs
    │   ├── Middleware/
    │   │   └── ExceptionHandlingMiddleware.cs
    │   ├── appsettings.json
    │   ├── Program.cs
    │   └── Properties/launchSettings.json
    │
    ├── Parking.Tests/
    │   ├── Handlers/
    │   │   ├── CreateUserCommandHandlerTests.cs
    │   │   ├── LoginCommandHandlerTests.cs
    │   │   └── CreateCompanyCommandHandlerTests.cs
    │   └── Validators/
    │       └── CreateUserCommandValidatorTests.cs
    │
    ├── Parking.Specs/
    │   ├── Features/
    │   │   ├── Authentication.feature
    │   │   └── Company.feature
    │   └── StepDefinitions/
    │       ├── AuthenticationSteps.cs
    │       └── CompanySteps.cs
    │
    ├── Parking.ArchTests/
    │   └── ArchitectureTests.cs
    │
    └── Parking.sln
```

---

## Tech Stack (Fase 1)

| Layer | Technology | Version |
|-------|-----------|---------|
| Runtime | .NET 9 / C# 13 | 9.0 |
| Web API | ASP.NET Core Web API | 9.0 |
| ORM | EF Core SQL Server | 9.0.5 |
| Messaging | MediatR | 12.4.1 |
| Validation | FluentValidation | 11.11.0 |
| Auth | JWT Bearer + BCrypt | 9.0.5 / 4.0.3 |
| Unit Tests | xUnit + FluentAssertions + NSubstitute | 2.9.2 / 6.12.2 / 5.3.0 |
| BDD Tests | Reqnroll + Moq | 2.4.1 / 4.20.72 |
| Arch Tests | NetArchTest.Rules | 1.3.2 |
| Database | SQL Server | T-SQL |

---

## Success Criteria

✅ **Functional:**
- Usuário consegue registrar conta (CreateUser)
- Usuário consegue fazer login (Login)
- Token refresh funciona (RefreshToken)
- Admin consegue criar Company/Branch
- Todos os handlers estão implementados
- Todos os tests passando (unit + arch)

✅ **Architecture:**
- Clean Architecture mantida (dependency flow)
- DDD patterns aplicados (aggregates, value objects)
- CQRS separado (commands vs queries)
- Result pattern (não exceptions para lógica)
- All handlers `internal sealed`
- All repositories `internal sealed`
- Domain: zero external dependencies

✅ **Database:**
- DDL script corre sem erros (idempotent)
- Seed data cria roles, permissions, example users
- Migrations EF Core criadas

---

## Próximas Ações

1. **Criar solução .NET 9** — `dotnet new sln Parking`
2. **Criar projects** — Domainm, Application, Infrastructure, API, Tests, Specs, ArchTests
3. **Instalar NuGets** — MediatR, EF Core, FluentValidation, xUnit, etc
4. **Scaffold base classes** — Entity, AggregateRoot, ValueObject, Error, Result
5. **Implementar Domain** — Entities, ValueObjects, Repositories
6. **Implementar Infrastructure** — AppDbContext, Configurations, Repositories, Services
7. **Implementar Application** — Commands, Queries, Handlers, Validators, Behaviors
8. **Implementar API** — Controllers, Middleware, Program.cs
9. **Implementar Tests** — Unit + BDD + Architecture

---

**Próximo Sprint:** Começar com Domain + Infrastructure (base classes + AppDbContext)

**Responsável:** Arquitetura SyncBar (DDD/CQRS)  
**Data Atualização:** 2026-07-15
