# 🚀 COMECE AQUI — Fase 1 Implementation Guide

**Status:** ✅ Todos os templates prontos para copiar-colar  
**Tempo estimado:** 2 sprints (10 dias)  
**Próximo passo:** Siga este guia linha por linha

---

## 📋 Você tem TUDO agora. Eis a ordem exata para começar:

### 1️⃣ Setup Scaffolding (2 horas)

Abra PowerShell/Terminal e execute **na pasta `/backend`**:

```bash
# Copie os comandos de SCAFFOLDING_FASE1.md seção "1️⃣ Setup Inicial"
# Crie solução + 7 projects + referências
dotnet new sln -n Parking
dotnet new classlib -n Parking.Domain
# ... (continue com os comandos do arquivo)

# Instale NuGets (seção "3️⃣ Instalar NuGets")
cd Parking.Domain && dotnet add package MediatR && cd ..
# ... (continue com os comandos)

# Crie pastas (seção "4️⃣ Criar Estrutura de Pastas")
mkdir Parking.Domain/Common
# ... (continue com mkdir commands)
```

**Arquivo de referência:** `SCAFFOLDING_FASE1.md`

---

### 2️⃣ Domain Layer — Base Classes (1 hora)

Copie estes 6 arquivos do `SCAFFOLDING_FASE1.md` seção "5️⃣":

- ✅ `Parking.Domain/Common/Entity.cs`
- ✅ `Parking.Domain/Common/AggregateRoot.cs`
- ✅ `Parking.Domain/Common/IDomainEvent.cs`
- ✅ `Parking.Domain/Common/ValueObject.cs`
- ✅ `Parking.Domain/Common/Error.cs`
- ✅ `Parking.Domain/Common/Result.cs`

Compile e verifique:
```bash
dotnet build Parking.Domain
```

---

### 3️⃣ Domain Layer — Entities + Value Objects + Repositories (4 horas)

**Use o arquivo:** `TEMPLATES_PARTE1_Domain_Infrastructure.cs`

Copie cada seção para seus arquivos:

**Entities (9 arquivos):**
- [ ] `Parking.Domain/Entities/Company.cs`
- [ ] `Parking.Domain/Entities/Branch.cs`
- [ ] `Parking.Domain/Entities/Role.cs`
- [ ] `Parking.Domain/Entities/Permission.cs`
- [ ] `Parking.Domain/Entities/AppUser.cs`
- [ ] `Parking.Domain/Entities/RefreshToken.cs`
- [ ] `Parking.Domain/Entities/UserRole.cs`
- [ ] `Parking.Domain/Entities/RolePermission.cs`
- [ ] `Parking.Domain/Entities/AccessLog.cs`

**Value Objects (3 arquivos):**
- [ ] `Parking.Domain/ValueObjects/Email.cs`
- [ ] `Parking.Domain/ValueObjects/Username.cs`
- [ ] `Parking.Domain/ValueObjects/PhoneNumber.cs`

**Repository Interfaces (6 arquivos):**
- [ ] `Parking.Domain/Repositories/ICompanyRepository.cs`
- [ ] `Parking.Domain/Repositories/IBranchRepository.cs`
- [ ] `Parking.Domain/Repositories/IAppUserRepository.cs`
- [ ] `Parking.Domain/Repositories/IRoleRepository.cs`
- [ ] `Parking.Domain/Repositories/IPermissionRepository.cs`
- [ ] `Parking.Domain/Repositories/IUnitOfWork.cs`

Compile:
```bash
dotnet build Parking.Domain
```

---

### 4️⃣ Infrastructure Layer (4 horas)

**Use o arquivo:** `TEMPLATES_PARTE1_Domain_Infrastructure.cs`

**EF Core Setup:**
- [ ] `Parking.Infrastructure/Persistence/AppDbContext.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/CompanyConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/BranchConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/RoleConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/PermissionConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/AppUserConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/RefreshTokenConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/UserRoleConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/RolePermissionConfiguration.cs`
- [ ] `Parking.Infrastructure/Persistence/Configurations/AccessLogConfiguration.cs`

**Repositories (5 arquivos):**
- [ ] `Parking.Infrastructure/Persistence/Repositories/CompanyRepository.cs`
- [ ] `Parking.Infrastructure/Persistence/Repositories/BranchRepository.cs`
- [ ] `Parking.Infrastructure/Persistence/Repositories/AppUserRepository.cs`
- [ ] `Parking.Infrastructure/Persistence/Repositories/RoleRepository.cs`
- [ ] `Parking.Infrastructure/Persistence/Repositories/PermissionRepository.cs`

**Services (3 arquivos):**
- [ ] `Parking.Infrastructure/Services/PasswordHasher.cs`
- [ ] `Parking.Infrastructure/Services/JwtTokenService.cs`
- [ ] `Parking.Infrastructure/Services/CurrentUserService.cs`

**DI Setup:**
- [ ] `Parking.Infrastructure/DependencyInjection.cs`

Compile:
```bash
dotnet build Parking.Infrastructure
```

---

### 5️⃣ Application Layer — Commands, Queries, DTOs (4 horas)

**Use o arquivo:** `TEMPLATES_PARTE2_Application_API_Tests.cs`

**Messaging Abstractions (6 interfaces):**
- [ ] `Parking.Application/Abstractions/Messaging/ICommand.cs`
- [ ] `Parking.Application/Abstractions/Messaging/ICommandHandler.cs`
- [ ] `Parking.Application/Abstractions/Messaging/IQuery.cs`
- [ ] `Parking.Application/Abstractions/Messaging/IQueryHandler.cs`

**Service Abstractions (3 interfaces):**
- [ ] `Parking.Application/Abstractions/Services/IPasswordHasher.cs`
- [ ] `Parking.Application/Abstractions/Services/ITokenService.cs`
- [ ] `Parking.Application/Abstractions/Services/ICurrentUser.cs`

**DTOs (4 arquivos):**
- [ ] `Parking.Application/Features/Common/DTOs/UserDto.cs`
- [ ] `Parking.Application/Features/Common/DTOs/RoleDto.cs`
- [ ] `Parking.Application/Features/Common/DTOs/CompanyDto.cs`
- [ ] `Parking.Application/Features/Common/DTOs/BranchDto.cs`

**Commands + Handlers + Validators (18 arquivos):**
- [ ] `Parking.Application/Features/Auth/CreateUser/CreateUserCommand.cs`
- [ ] `Parking.Application/Features/Auth/CreateUser/CreateUserCommandHandler.cs`
- [ ] `Parking.Application/Features/Auth/CreateUser/CreateUserCommandValidator.cs`
- [ ] `Parking.Application/Features/Auth/Login/LoginCommand.cs`
- [ ] `Parking.Application/Features/Auth/Login/LoginCommandHandler.cs`
- [ ] `Parking.Application/Features/Auth/Login/LoginCommandValidator.cs`
- [ ] `Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommand.cs`
- [ ] `Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommandHandler.cs`
- [ ] `Parking.Application/Features/Auth/RefreshToken/RefreshTokenCommandValidator.cs`
- [ ] `Parking.Application/Features/Auth/AssignRole/AssignRoleCommand.cs`
- [ ] `Parking.Application/Features/Auth/AssignRole/AssignRoleCommandHandler.cs`
- [ ] `Parking.Application/Features/Auth/AssignRole/AssignRoleCommandValidator.cs`
- [ ] `Parking.Application/Features/Company/Create/CreateCompanyCommand.cs`
- [ ] `Parking.Application/Features/Company/Create/CreateCompanyCommandHandler.cs`
- [ ] `Parking.Application/Features/Company/Create/CreateCompanyCommandValidator.cs`
- [ ] `Parking.Application/Features/Branch/Create/CreateBranchCommand.cs`
- [ ] `Parking.Application/Features/Branch/Create/CreateBranchCommandHandler.cs`
- [ ] `Parking.Application/Features/Branch/Create/CreateBranchCommandValidator.cs`

**Queries + Handlers (8 arquivos):**
- [ ] `Parking.Application/Features/Auth/GetUsers/GetAllUsersQuery.cs`
- [ ] `Parking.Application/Features/Auth/GetUsers/GetAllUsersQueryHandler.cs`
- [ ] `Parking.Application/Features/Company/GetById/GetCompanyByIdQuery.cs`
- [ ] `Parking.Application/Features/Company/GetById/GetCompanyByIdQueryHandler.cs`
- [ ] `Parking.Application/Features/Branch/GetByCompany/GetBranchesByCompanyQuery.cs`
- [ ] `Parking.Application/Features/Branch/GetByCompany/GetBranchesByCompanyQueryHandler.cs`

**Behaviors + DI (3 arquivos):**
- [ ] `Parking.Application/Behaviors/LoggingBehavior.cs`
- [ ] `Parking.Application/Behaviors/ValidationBehavior.cs`
- [ ] `Parking.Application/DependencyInjection.cs`

Compile:
```bash
dotnet build Parking.Application
```

---

### 6️⃣ API Layer (2 horas)

**Use o arquivo:** `TEMPLATES_PARTE2_Application_API_Tests.cs` + `TEMPLATES_Program_Appsettings.cs`

**Controllers (4 arquivos):**
- [ ] `Parking.API/Controllers/ApiController.cs`
- [ ] `Parking.API/Controllers/AuthController.cs`
- [ ] `Parking.API/Controllers/CompanyController.cs`
- [ ] `Parking.API/Controllers/BranchController.cs`

**Middleware:**
- [ ] `Parking.API/Middleware/ExceptionHandlingMiddleware.cs`

**Configuration:**
- [ ] `Parking.API/Program.cs` (copie de TEMPLATES_Program_Appsettings.cs)
- [ ] `Parking.API/appsettings.json` (copie de TEMPLATES_Program_Appsettings.cs)
- [ ] `Parking.API/appsettings.Development.json` (copie de TEMPLATES_Program_Appsettings.cs)

**Importante:** Atualize sua connection string e JWT secret!

Compile:
```bash
dotnet build Parking.API
```

---

### 7️⃣ Database Migrations (30 min)

```bash
# Crie migration
dotnet ef migrations add Initial --project Parking.Infrastructure --startup-project Parking.API

# Aplique migration
dotnet ef database update --project Parking.Infrastructure --startup-project Parking.API
```

---

### 8️⃣ Tests (2 horas)

**Use o arquivo:** `TEMPLATES_PARTE2_Application_API_Tests.cs`

**Unit Tests (4 arquivos):**
- [ ] `Parking.Tests/Handlers/CreateUserCommandHandlerTests.cs`
- [ ] `Parking.Tests/Handlers/LoginCommandHandlerTests.cs`
- [ ] `Parking.Tests/Handlers/CreateCompanyCommandHandlerTests.cs`
- [ ] `Parking.Tests/Handlers/GetAllUsersQueryHandlerTests.cs`

**BDD Features (2 arquivos):**
- [ ] `Parking.Specs/Features/Authentication.feature`
- [ ] `Parking.Specs/Features/Company.feature`

**BDD Step Definitions (2 arquivos):**
- [ ] `Parking.Specs/StepDefinitions/AuthenticationSteps.cs`
- [ ] `Parking.Specs/StepDefinitions/CompanySteps.cs`

**Architecture Tests:**
- [ ] `Parking.ArchTests/ArchitectureTests.cs`

Rode os testes:
```bash
dotnet test
```

---

## ✅ Verify Everything Works

```bash
# Build
dotnet build

# Test
dotnet test

# Run API
dotnet run --project Parking.API
```

A API estará em: **https://localhost:7001**

Teste um endpoint:
```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"userName":"test","email":"test@example.com","fullName":"Test User","password":"Password123"}'
```

---

## 📚 Referência Rápida — Onde Copiar Cada Coisa

| Arquivo Que Você Precisa | Copiar De |
|--------------------------|-----------|
| Entity.cs, AggregateRoot.cs, etc | `SCAFFOLDING_FASE1.md` seção 5 |
| Company, Branch, AppUser, etc | `TEMPLATES_PARTE1_Domain_Infrastructure.cs` |
| AppDbContext, Configurations, Repositories | `TEMPLATES_PARTE1_Domain_Infrastructure.cs` |
| PasswordHasher, JwtTokenService, CurrentUserService | `TEMPLATES_PARTE1_Domain_Infrastructure.cs` |
| CreateUserCommand, LoginCommand, etc | `TEMPLATES_PARTE2_Application_API_Tests.cs` |
| AuthController, CompanyController, etc | `TEMPLATES_PARTE2_Application_API_Tests.cs` |
| ExceptionHandlingMiddleware | `TEMPLATES_PARTE2_Application_API_Tests.cs` |
| Program.cs, appsettings.json | `TEMPLATES_Program_Appsettings.cs` |
| Unit Tests | `TEMPLATES_PARTE2_Application_API_Tests.cs` |
| BDD Features & Steps | `TEMPLATES_PARTE2_Application_API_Tests.cs` |
| Architecture Tests | `TEMPLATES_PARTE2_Application_API_Tests.cs` |

---

## 🎯 Success Checklist (End of Fase 1)

- [ ] `dotnet build` compila sem erros
- [ ] `dotnet ef database update` cria banco sem erros
- [ ] `dotnet test` — todos os testes passam
- [ ] `dotnet run --project Parking.API` inicia sem erros
- [ ] POST /api/auth/register funciona
- [ ] POST /api/auth/login retorna JWT token
- [ ] GET /api/auth/users retorna lista de usuários
- [ ] POST /api/companies cria company
- [ ] POST /api/branches cria branch
- [ ] Todos os testes de arquitetura passam

---

## ❓ Troubleshooting

**"Class 'X' is missing Entity or AggregateRoot base?"**
→ Verifique que copiou os base classes corretamente de SCAFFOLDING_FASE1.md

**"Cannot add DbSet — type not recognized?"**
→ Verifique que criou as EF Configurations e chamou `ApplyConfigurationsFromAssembly()` em OnModelCreating

**"JWT token says 'secret key must be 32 chars'?"**
→ Atualize appsettings.json com uma secret key maior

**"Connection string error?"**
→ Atualize ConnectionString em appsettings.json com seu SQL Server details

**"Tests failing with 'NSubstitute not found'?"**
→ Rode `dotnet add package NSubstitute` no projeto Tests

---

## 🚀 Pronto?

Você agora tem:
- ✅ Todos os templates prontos para copiar-colar
- ✅ Passo a passo detalhado
- ✅ Exemplos de código em cada camada
- ✅ Testes já modelados
- ✅ Configuração de DI
- ✅ Arquitetura limpa validada

**Próximo passo:** Comece com a seção "1️⃣ Setup Scaffolding" acima!

**Estimado:** 10 dias até MVP da Fase 1 ✨

