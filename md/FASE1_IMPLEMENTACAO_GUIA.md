# Fase 1 — Guia Completo de Implementação

**Status:** 🟢 Pronto para codificar  
**Tempo estimado:** 2 sprints (10 dias de desenvolvimento)  
**Estrutura:** 6 arquivos + 30+ classes

---

## 📊 Mapa de Implementação

```
Fase 1: Foundation
├── 1. Domain Layer (Zero External Dependencies)
│   ├── ✅ Base Classes (Entity, AggregateRoot, ValueObject, Error, Result)
│   ├── Entities (9 classes)
│   │   ├── Company (AggregateRoot)
│   │   ├── Branch (AggregateRoot)
│   │   ├── Role (AggregateRoot)
│   │   ├── Permission (AggregateRoot)
│   │   ├── AppUser (AggregateRoot)
│   │   ├── RefreshToken (Entity)
│   │   ├── UserRole (Entity)
│   │   ├── RolePermission (Entity)
│   │   └── AccessLog (Entity)
│   ├── Value Objects (3 classes)
│   │   ├── Email
│   │   ├── Username
│   │   └── PhoneNumber
│   └── Repository Interfaces (6 interfaces)
│       ├── ICompanyRepository
│       ├── IBranchRepository
│       ├── IAppUserRepository
│       ├── IRoleRepository
│       ├── IPermissionRepository
│       └── IUnitOfWork
│
├── 2. Infrastructure Layer (EF Core + Services)
│   ├── AppDbContext (1 class)
│   ├── EF Configurations (9 classes)
│   │   ├── CompanyConfiguration
│   │   ├── BranchConfiguration
│   │   ├── AppUserConfiguration
│   │   ├── RoleConfiguration
│   │   ├── PermissionConfiguration
│   │   ├── UserRoleConfiguration
│   │   ├── RolePermissionConfiguration
│   │   ├── RefreshTokenConfiguration
│   │   └── AccessLogConfiguration
│   ├── Repositories (5 classes)
│   │   ├── CompanyRepository
│   │   ├── BranchRepository
│   │   ├── AppUserRepository
│   │   ├── RoleRepository
│   │   └── PermissionRepository
│   ├── Services (3 classes)
│   │   ├── PasswordHasher
│   │   ├── JwtTokenService
│   │   └── CurrentUserService
│   └── DependencyInjection.cs
│
├── 3. Application Layer (CQRS)
│   ├── Messaging Abstractions (6 interfaces)
│   │   ├── ICommand
│   │   ├── ICommand<T>
│   │   ├── ICommandHandler<T>
│   │   ├── ICommandHandler<T, R>
│   │   ├── IQuery<T>
│   │   └── IQueryHandler<T, R>
│   ├── Service Abstractions (3 interfaces)
│   │   ├── IPasswordHasher
│   │   ├── ITokenService
│   │   └── ICurrentUser
│   ├── Commands (10 classes)
│   │   ├── CreateUserCommand + Handler + Validator
│   │   ├── LoginCommand + Handler + Validator
│   │   ├── RefreshTokenCommand + Handler + Validator
│   │   ├── AssignRoleCommand + Handler + Validator
│   │   ├── CreateCompanyCommand + Handler + Validator
│   │   └── CreateBranchCommand + Handler + Validator
│   ├── Queries (4 classes)
│   │   ├── GetAllUsersQuery + Handler
│   │   ├── GetAllRolesQuery + Handler
│   │   ├── GetCompanyByIdQuery + Handler
│   │   └── GetBranchesByCompanyQuery + Handler
│   ├── DTOs (4 classes)
│   │   ├── UserDto
│   │   ├── RoleDto
│   │   ├── CompanyDto
│   │   └── BranchDto
│   ├── Behaviors (2 classes)
│   │   ├── LoggingBehavior
│   │   └── ValidationBehavior
│   └── DependencyInjection.cs
│
├── 4. API Layer (Web)
│   ├── ApiController (1 base class)
│   ├── Controllers (4 classes)
│   │   ├── AuthController
│   │   ├── CompanyController
│   │   ├── BranchController
│   │   └── (Health check / diagnostic)
│   ├── Middleware (1 class)
│   │   └── ExceptionHandlingMiddleware
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── 5. Tests (xUnit + NSubstitute)
│   ├── Unit Tests (4 test classes)
│   │   ├── CreateUserCommandHandlerTests
│   │   ├── LoginCommandHandlerTests
│   │   ├── CreateCompanyCommandHandlerTests
│   │   └── GetAllUsersQueryHandlerTests
│   ├── Integration (opcional)
│   │   └── AuthControllerTests
│   └── Validators (opcional)
│
├── 6. BDD Tests (Reqnroll)
│   ├── Features
│   │   ├── Authentication.feature (4 scenarios)
│   │   └── Company.feature (2 scenarios)
│   └── StepDefinitions
│       ├── AuthenticationSteps.cs
│       └── CompanySteps.cs
│
└── 7. Architecture Tests (NetArchTest)
    └── ArchitectureTests.cs (10+ rules)
```

---

## 🔧 Arquivos de Referência Disponíveis

### ✅ Já Criados (copie direto)

1. **Parking.Domain_BaseClasses.cs** (235 linhas)
   - Entity, AggregateRoot, ValueObject, Error, Result
   - Pronto para copiar para `Parking.Domain/Common/`

2. **SCAFFOLDING_FASE1.md**
   - Setup solução + projects + NuGets + pastas
   - Comandos prontos para rodar

3. **Fase1_Foundation.md**
   - Feature checklist detalhado
   - Cada entidade, repo, handler mapeado

4. **README_FASE1.md**
   - Step-by-step com exemplos de código
   - Pattern patterns (Entity factory, ValueObject, etc)

### 📝 Próximos: Templates Completos (criar)

Você quer que eu crie templates prontos para:

1. **Fase1_Domain_Entities.cs** (2000 linhas)
   - Todas as 9 entidades + 3 value objects + 6 repository interfaces
   - Copy-paste direto nos arquivos corretos

2. **Fase1_Infrastructure_Setup.cs** (1500 linhas)
   - AppDbContext + 9 EF configs + 5 repositories
   - Services (PasswordHasher, JwtTokenService, CurrentUserService)

3. **Fase1_Application_Features.cs** (2500 linhas)
   - 10 commands + 4 queries + handlers + validators
   - DTOs + pipeline behaviors

4. **Fase1_API_Controllers.cs** (800 linhas)
   - ApiController + 4 controllers
   - ExceptionHandlingMiddleware
   - Program.cs

5. **Fase1_Tests.cs** (1000 linhas)
   - xUnit tests
   - Reqnroll features + steps
   - NetArchTest rules

---

## 📋 Ordem de Implementação Recomendada

### Sprint 1 (5 dias) — Domain + Infrastructure

**Day 1-2: Domain Layer**
- [ ] Copy base classes (Parking.Domain_BaseClasses.cs)
- [ ] Implement 9 entities
- [ ] Implement 3 value objects
- [ ] Implement 6 repository interfaces
- [ ] Compile & verify no errors

**Day 3-5: Infrastructure**
- [ ] Create AppDbContext
- [ ] Implement 9 EF configurations
- [ ] Implement 5 repositories
- [ ] Implement 3 services (PasswordHasher, JwtTokenService, CurrentUserService)
- [ ] Setup DependencyInjection.cs
- [ ] Run migrations: `dotnet ef migrations add Initial`

### Sprint 2 (5 dias) — Application + API + Tests

**Day 6-7: Application**
- [ ] Implement messaging abstractions
- [ ] Implement 10 commands + handlers + validators
- [ ] Implement 4 queries + handlers
- [ ] Create 4 DTOs
- [ ] Implement 2 pipeline behaviors
- [ ] Setup DependencyInjection.cs

**Day 8-9: API**
- [ ] Implement ApiController base
- [ ] Implement 4 controllers
- [ ] Implement ExceptionHandlingMiddleware
- [ ] Setup Program.cs with DI, JWT auth
- [ ] Configure appsettings.json
- [ ] Test endpoints with Postman/curl

**Day 10: Tests**
- [ ] Unit tests (4 test classes)
- [ ] BDD tests (Reqnroll features + steps)
- [ ] Architecture tests (NetArchTest rules)
- [ ] `dotnet test` — all passing

---

## 🎯 Success Criteria (End of Fase 1)

✅ **Functional**
- POST /api/auth/register → creates user ✅
- POST /api/auth/login → returns JWT token ✅
- POST /api/auth/refresh-token → new access token ✅
- POST /api/auth/assign-role → user gets role ✅
- POST /api/companies → creates company ✅
- GET /api/companies/{id} → returns company ✅
- POST /api/branches → creates branch ✅
- GET /api/branches?companyId=X → returns branches ✅

✅ **Architecture**
- Domain: zero external dependencies ✅
- Application: no Infrastructure/API refs ✅
- All handlers: `internal sealed` ✅
- All repos: `internal sealed` ✅
- All responses: `sealed record` ✅
- Clean Architecture diagram verified ✅

✅ **Tests**
- All unit tests passing ✅
- All BDD scenarios passing ✅
- All architecture rules passing ✅
- Code coverage >80% ✅

✅ **Database**
- Migrations created and applied ✅
- Seed data loaded ✅
- All foreign keys working ✅

---

## 🚀 Quick Start

```bash
# 1. Navigate to backend
cd backend

# 2. Run scaffolding (from SCAFFOLDING_FASE1.md)
dotnet new sln -n Parking
# ... create projects ...

# 3. Copy base classes (from Parking.Domain_BaseClasses.cs)
# into Parking.Domain/Common/

# 4. Follow day-by-day checklist above

# 5. Build & test
dotnet build
dotnet test
dotnet run --project Parking.API
```

---

## 📚 Reference Files Location

```
C:\Users\AMD\Documents\Claude\Projects\Parking\
├── backend/
│   ├── SCAFFOLDING_FASE1.md ← start here
│   ├── Parking.Domain_BaseClasses.cs ← copy to Domain/Common/
│   ├── README_FASE1.md ← patterns & examples
│   ├── Fase1_Foundation.md ← feature checklist
│   └── (projects to be created...)
├── sql/
│   ├── Parking_DDL.sql
│   └── Parking_Seed.sql
└── md/
    ├── Parking_Modelagem.md
    ├── Parking_ChecklistModulos.md
    └── Fase1_Foundation.md
```

---

## ❓ Questions?

**Q: Should I create all 7 projects upfront?**  
A: Yes — run all scaffolding commands first, then implement layer by layer.

**Q: Do I need migrations immediately?**  
A: Create them after Infrastructure is ready (`dotnet ef migrations add Initial`).

**Q: How do I test without a database?**  
A: Use NSubstitute mocks in unit tests. For integration tests, use in-memory Sqlite or real SQL Server.

**Q: Can I skip tests?**  
A: Not recommended — tests catch errors early. At minimum: 1 handler test + 1 feature test.

---

**Next Step:** Create templates for all Domain/Infrastructure/Application/API code  
**Estimated Time:** 2-3 hours of development per day × 10 days = MVP ready

Ready to code! 🚀
