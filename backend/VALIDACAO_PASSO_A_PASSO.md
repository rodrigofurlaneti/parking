# ✅ Checklist de Validação — Fase 1

Use este arquivo para validar que cada etapa foi implementada corretamente.

---

## 1️⃣ Scaffolding & NuGets

Rode estes comandos na pasta `/backend`:

```bash
# Verificar solução criada
ls -la *.sln
# ✅ Deve existir: Parking.sln

# Verificar projects
ls -d Parking.*
# ✅ Deve existir 7 pastas:
# Parking.Domain
# Parking.Application
# Parking.Infrastructure
# Parking.API
# Parking.Tests
# Parking.Specs
# Parking.ArchTests

# Verificar pastas criadas
ls Parking.Domain/
# ✅ Deve conter: Common, Entities, ValueObjects, Repositories

# Compilar
dotnet build
# ✅ 0 erros, 0 warnings (ou warnings aceitáveis)
```

---

## 2️⃣ Domain Layer

```bash
# Verificar arquivos base classes
ls -la Parking.Domain/Common/
# ✅ Deve existir 6 arquivos:
# Entity.cs
# AggregateRoot.cs
# IDomainEvent.cs
# ValueObject.cs
# Error.cs
# Result.cs

# Compilar apenas Domain
dotnet build Parking.Domain
# ✅ Build successful

# Verificar namespaces
grep -r "namespace Parking.Domain.Common" Parking.Domain/Common/
# ✅ Deve encontrar 6 resultados

# Verificar Entity tem IEquatable
grep "IEquatable<Entity>" Parking.Domain/Common/Entity.cs
# ✅ Deve encontrar 1 resultado
```

---

## 3️⃣ Domain Entities

```bash
# Contar entidades
ls Parking.Domain/Entities/
# ✅ Deve existir 9 arquivos:
# Company.cs, Branch.cs, Role.cs, Permission.cs, AppUser.cs,
# RefreshToken.cs, UserRole.cs, RolePermission.cs, AccessLog.cs

# Verificar que herdam de AggregateRoot
grep "class.*: AggregateRoot" Parking.Domain/Entities/*.cs | wc -l
# ✅ Deve contar 5 ou mais (Company, Branch, Role, Permission, AppUser são aggregates)

# Verificar AppUser tem Email e Username
grep "Email\|Username" Parking.Domain/Entities/AppUser.cs
# ✅ Deve encontrar 2+ resultados

# Verificar Company tem método Create() que retorna Result
grep "Create.*Result<Company>" Parking.Domain/Entities/Company.cs
# ✅ Deve encontrar 1 resultado
```

---

## 4️⃣ Domain Value Objects

```bash
# Contar value objects
ls Parking.Domain/ValueObjects/
# ✅ Deve existir 3 arquivos:
# Email.cs, Username.cs, PhoneNumber.cs

# Verificar que herdam de ValueObject
grep "class.*: ValueObject" Parking.Domain/ValueObjects/*.cs | wc -l
# ✅ Deve contar 3

# Verificar Email tem regex validation
grep "Regex\|EmailRegex" Parking.Domain/ValueObjects/Email.cs
# ✅ Deve encontrar 2+ resultados

# Verificar Username tem Create factory
grep "Create.*Result<Username>" Parking.Domain/ValueObjects/Username.cs
# ✅ Deve encontrar 1 resultado
```

---

## 5️⃣ Domain Repository Interfaces

```bash
# Contar interfaces
ls Parking.Domain/Repositories/
# ✅ Deve existir 6 arquivos:
# ICompanyRepository.cs, IBranchRepository.cs, IAppUserRepository.cs,
# IRoleRepository.cs, IPermissionRepository.cs, IUnitOfWork.cs

# Verificar que são interfaces
grep "interface I" Parking.Domain/Repositories/I*.cs | wc -l
# ✅ Deve contar 6

# Verificar IUnitOfWork tem CommitAsync
grep "CommitAsync" Parking.Domain/Repositories/IUnitOfWork.cs
# ✅ Deve encontrar 1 resultado

# Compilar Domain novamente
dotnet build Parking.Domain
# ✅ Build successful (0 errors)
```

---

## 6️⃣ Infrastructure Layer Setup

```bash
# Verificar AppDbContext existe
ls Parking.Infrastructure/Persistence/AppDbContext.cs
# ✅ Deve existir

# Contar EF Configurations
ls Parking.Infrastructure/Persistence/Configurations/
# ✅ Deve existir 9 arquivos (one per entity)

# Verificar AppDbContext herda de DbContext e implementa IUnitOfWork
grep "DbContext.*IUnitOfWork\|class AppDbContext" Parking.Infrastructure/Persistence/AppDbContext.cs
# ✅ Deve encontrar implementação

# Verificar CompanyConfiguration usa IEntityTypeConfiguration
grep "IEntityTypeConfiguration<Company>" Parking.Infrastructure/Persistence/Configurations/CompanyConfiguration.cs
# ✅ Deve encontrar 1 resultado

# Verificar repositories existem
ls Parking.Infrastructure/Persistence/Repositories/
# ✅ Deve existir 5 arquivos (CompanyRepository, BranchRepository, AppUserRepository, RoleRepository, PermissionRepository)

# Verificar services existem
ls Parking.Infrastructure/Services/
# ✅ Deve existir 3 arquivos:
# PasswordHasher.cs, JwtTokenService.cs, CurrentUserService.cs

# Verificar PasswordHasher usa BCrypt
grep "BCrypt\|workFactor" Parking.Infrastructure/Services/PasswordHasher.cs
# ✅ Deve encontrar referências a BCrypt

# Verificar JwtTokenService cria tokens
grep "SymmetricSecurityKey\|JwtSecurityToken" Parking.Infrastructure/Services/JwtTokenService.cs
# ✅ Deve encontrar 2+ resultados

# Compilar Infrastructure
dotnet build Parking.Infrastructure
# ✅ Build successful (0 errors)
```

---

## 7️⃣ Application Layer Setup

```bash
# Verificar messaging abstractions
ls Parking.Application/Abstractions/Messaging/
# ✅ Deve existir 4 arquivos:
# ICommand.cs, ICommandHandler.cs, IQuery.cs, IQueryHandler.cs

# Verificar service abstractions
ls Parking.Application/Abstractions/Services/
# ✅ Deve existir 3 arquivos:
# IPasswordHasher.cs, ITokenService.cs, ICurrentUser.cs

# Verificar DTOs
ls Parking.Application/Features/Common/DTOs/
# ✅ Deve existir 4 arquivos:
# UserDto.cs, RoleDto.cs, CompanyDto.cs, BranchDto.cs

# Verificar commands/queries estrutura
find Parking.Application/Features -name "*Command.cs" | wc -l
# ✅ Deve contar 6+ (CreateUser, Login, RefreshToken, AssignRole, CreateCompany, CreateBranch)

find Parking.Application/Features -name "*Query.cs" | wc -l
# ✅ Deve contar 4+ (GetAllUsers, GetCompanyById, GetBranchesByCompany)

# Verificar handlers
find Parking.Application/Features -name "*Handler.cs" | wc -l
# ✅ Deve contar 10+ (6 command handlers + 4 query handlers)

# Verificar validators
find Parking.Application/Features -name "*Validator.cs" | wc -l
# ✅ Deve contar 6+ (validators para commands)

# Verificar behaviors
ls Parking.Application/Behaviors/
# ✅ Deve existir 2 arquivos:
# LoggingBehavior.cs, ValidationBehavior.cs

# Verificar DependencyInjection.cs
grep "AddMediatR\|AddValidators\|AddTransient" Parking.Application/DependencyInjection.cs
# ✅ Deve encontrar 3+ resultados

# Compilar Application
dotnet build Parking.Application
# ✅ Build successful (0 errors)
```

---

## 8️⃣ API Layer Setup

```bash
# Verificar controllers
ls Parking.API/Controllers/
# ✅ Deve existir 4+ arquivos:
# ApiController.cs, AuthController.cs, CompanyController.cs, BranchController.cs

# Verificar ApiController tem HandleFailure
grep "HandleFailure" Parking.API/Controllers/ApiController.cs
# ✅ Deve encontrar 2+ resultados (método overload)

# Verificar cada controller herda de ApiController
grep "class.*Controller.*ApiController" Parking.API/Controllers/*Controller.cs | wc -l
# ✅ Deve contar 3 (Auth, Company, Branch)

# Verificar middleware
ls Parking.API/Middleware/ExceptionHandlingMiddleware.cs
# ✅ Deve existir

# Verificar Program.cs tem AddApplication() e AddInfrastructure()
grep "AddApplication\|AddInfrastructure" Parking.API/Program.cs
# ✅ Deve encontrar 2 resultados

# Verificar appsettings.json tem ConnectionString e Jwt
grep "ConnectionStrings\|Jwt" Parking.API/appsettings.json
# ✅ Deve encontrar 2 resultados

# Compilar API
dotnet build Parking.API
# ✅ Build successful (0 errors)
```

---

## 9️⃣ Migrations & Database

```bash
# Criar migration
dotnet ef migrations add Initial --project Parking.Infrastructure --startup-project Parking.API
# ✅ Migration created: "Initial"

# Verificar migration foi criada
ls Parking.Infrastructure/Migrations/
# ✅ Deve existir pasta com timestamp + "_Initial.cs"

# Atualizar banco (certifique que ConnectionString está correta!)
dotnet ef database update --project Parking.Infrastructure --startup-project Parking.API
# ✅ 0 errors, database updated

# Verificar banco foi criado no SQL Server
# No SQL Server Management Studio:
# - Procure por banco: ParkingDb
# ✅ Deve existir banco com tabelas: Companies, Branches, AppUsers, Roles, Permissions, etc.
```

---

## 🔟 Tests

```bash
# Contar testes
find Parking.Tests -name "*Tests.cs" | wc -l
# ✅ Deve contar 4+ (CreateUserCommandHandlerTests, LoginCommandHandlerTests, CreateCompanyCommandHandlerTests, GetAllUsersQueryHandlerTests)

# Verificar BDD features
ls Parking.Specs/Features/
# ✅ Deve existir 2 arquivos:
# Authentication.feature, Company.feature

# Verificar BDD step definitions
ls Parking.Specs/StepDefinitions/
# ✅ Deve existir 2 arquivos:
# AuthenticationSteps.cs, CompanySteps.cs

# Verificar ArchTests
ls Parking.ArchTests/ArchitectureTests.cs
# ✅ Deve existir

# Compilar Tests
dotnet build Parking.Tests
# ✅ Build successful

# Rodar testes
dotnet test
# ✅ Tests should run (pode falhar alguns, é ok na primeira vez)
```

---

## 1️⃣1️⃣ Clean Architecture Validation

```bash
# Verificar Domain tem zero external dependencies
grep -r "using MediatR\|using Microsoft.EntityFrameworkCore\|using FluentValidation" Parking.Domain/
# ✅ Não deve encontrar nada (0 results)

# Verificar Application não referencia Infrastructure
grep -r "Parking.Infrastructure" Parking.Application/
# ✅ Não deve encontrar nada (0 results)

# Verificar Application não referencia API
grep -r "Parking.API" Parking.Application/
# ✅ Não deve encontrar nada (0 results)

# Verificar Handlers são internal sealed
grep "internal sealed class.*Handler" Parking.Application/Features -r | wc -l
# ✅ Deve contar 10+ (todos os handlers)

# Verificar Repositories são internal sealed
grep "internal sealed class.*Repository" Parking.Infrastructure -r | wc -l
# ✅ Deve contar 5 (todos os repositories)

# Verificar DTOs são sealed record
grep "sealed record.*Dto" Parking.Application/Features/Common/DTOs/ | wc -l
# ✅ Deve contar 4 (UserDto, RoleDto, CompanyDto, BranchDto)
```

---

## 1️⃣2️⃣ Full Build & Run

```bash
# Build completo
dotnet build
# ✅ Build successful (0 errors)

# Rodar testes
dotnet test
# ✅ Tests completed (pode ter alguns failures, ok)

# Rodar API
dotnet run --project Parking.API
# ✅ API started
# ✅ Log shows: "Now listening on: https://localhost:7001"

# Em outro terminal, testar endpoint
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"userName":"test","email":"test@example.com","fullName":"Test User","password":"Password123"}'

# ✅ Response: user created (ou validation error if fields are wrong)
```

---

## 🎯 Final Validation Checklist

- [ ] `dotnet build` compila sem erros
- [ ] `dotnet test` roda (mesmo que alguns testes falhem)
- [ ] `dotnet ef database update` cria banco sem erros
- [ ] `dotnet run --project Parking.API` inicia sem erros
- [ ] API responde em https://localhost:7001
- [ ] POST /api/auth/register funciona
- [ ] Banco tem 9 tabelas principais (Companies, Branches, AppUsers, Roles, Permissions, RefreshTokens, UserRoles, RolePermissions, AccessLogs)
- [ ] Domain zero external dependencies ✅
- [ ] Application não referencia Infrastructure ✅
- [ ] Todos os handlers são `internal sealed` ✅
- [ ] Todos os repositories são `internal sealed` ✅
- [ ] Todos os DTOs são `sealed record` ✅

---

## ❓ Se algo falhar...

### "Type 'X' not found"
→ Verifique que copiou o arquivo inteiro do template e está no namespace correto

### "Cannot resolve dependency 'Y'"
→ Verificar se DependencyInjection.cs tem todas as registrações em AddApplication() e AddInfrastructure()

### "Migration failed"
→ Verifique ConnectionString em appsettings.json e que SQL Server está rodando

### "API não inicia"
→ Verifique Program.cs tem todas as extensões: AddApplication(), AddInfrastructure(), AddControllers()

### "Tests fail with NSubstitute error"
→ Rode: `dotnet add package NSubstitute --version 5.1.1` em Parking.Tests

---

## ✨ Quando Tudo Funciona

Se chegou aqui com todos os checkboxes verdes:

```
🎉 Parabéns! Você completou Fase 1!
✅ Clean Architecture implementado
✅ Auth/Org foundation pronto
✅ Testes passando
✅ Banco funcionando
```

**Próximo passo:** Comece Fase 2 — Operacional (Employees + Cash + Parking Spaces)

