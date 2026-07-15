# Fase 1 — Execução Passo-a-Passo Detalhada

**Tempo Total:** 10 dias | **Sprints:** 2  
**Status:** 🟢 Pronto para começar  
**Complexidade:** 🟠 Média-Alta

---

## 📋 Dia 1-2: Setup + Domain Base Classes

### Dia 1: Scaffolding Solução .NET (2 horas)

```bash
cd backend

# 1. Criar solução
dotnet new sln -n Parking

# 2. Criar projects (7 projects)
dotnet new classlib -n Parking.Domain
dotnet new classlib -n Parking.Application
dotnet new classlib -n Parking.Infrastructure
dotnet new webapi -n Parking.API
dotnet new xunit -n Parking.Tests
dotnet new classlib -n Parking.Specs
dotnet new classlib -n Parking.ArchTests

# 3. Adicionar à solução
dotnet sln add Parking.Domain
dotnet sln add Parking.Application
dotnet sln add Parking.Infrastructure
dotnet sln add Parking.API
dotnet sln add Parking.Tests
dotnet sln add Parking.Specs
dotnet sln add Parking.ArchTests

# 4. Criar referências
# Application → Domain
cd Parking.Application && dotnet add reference ../Parking.Domain && cd ..

# Infrastructure → Domain
cd Parking.Infrastructure && dotnet add reference ../Parking.Domain && cd ..

# API → Application + Infrastructure
cd Parking.API && dotnet add reference ../Parking.Application ../Parking.Infrastructure && cd ..

# Tests → Application + Infrastructure
cd Parking.Tests && dotnet add reference ../Parking.Application ../Parking.Infrastructure && cd ..

# Specs → Application
cd Parking.Specs && dotnet add reference ../Parking.Application && cd ..

# ArchTests → todos
cd Parking.ArchTests && dotnet add reference ../Parking.Domain ../Parking.Application ../Parking.Infrastructure ../Parking.API && cd ..

# 5. Build (deve compilar sem erros)
dotnet build
```

### Dia 1: Instalar NuGets (1 hora)

```bash
# Parking.Domain
cd Parking.Domain
dotnet add package MediatR --version 12.4.1
cd ..

# Parking.Application
cd Parking.Application
dotnet add package MediatR --version 12.4.1
dotnet add package FluentValidation --version 11.11.0
cd ..

# Parking.Infrastructure
cd Parking.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.5
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next
cd ..

# Parking.API
cd Parking.API
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
cd ..

# Parking.Tests
cd Parking.Tests
dotnet add package xUnit
dotnet add package FluentAssertions
dotnet add package NSubstitute
cd ..

# Parking.Specs
cd Parking.Specs
dotnet add package Reqnroll
cd ..

# Parking.ArchTests
cd Parking.ArchTests
dotnet add package NetArchTest.Rules
cd ..
```

### Dia 1: Criar Estrutura de Pastas (30 min)

```bash
# Execute estes comandos na raiz do backend/

# Domain
mkdir Parking.Domain\Common
mkdir Parking.Domain\Entities
mkdir Parking.Domain\ValueObjects
mkdir Parking.Domain\Repositories

# Application
mkdir Parking.Application\Abstractions
mkdir Parking.Application\Abstractions\Messaging
mkdir Parking.Application\Abstractions\Services
mkdir Parking.Application\Features
mkdir Parking.Application\Features\Auth
mkdir Parking.Application\Features\Auth\CreateUser
mkdir Parking.Application\Features\Auth\Login
mkdir Parking.Application\Features\Auth\RefreshToken
mkdir Parking.Application\Features\Auth\AssignRole
mkdir Parking.Application\Features\Auth\GetUsers
mkdir Parking.Application\Features\Company
mkdir Parking.Application\Features\Company\Create
mkdir Parking.Application\Features\Company\GetById
mkdir Parking.Application\Features\Branch
mkdir Parking.Application\Features\Branch\Create
mkdir Parking.Application\Features\Branch\GetById
mkdir Parking.Application\Features\Common
mkdir Parking.Application\Features\Common\DTOs
mkdir Parking.Application\Behaviors

# Infrastructure
mkdir Parking.Infrastructure\Persistence
mkdir Parking.Infrastructure\Persistence\Configurations
mkdir Parking.Infrastructure\Persistence\Repositories
mkdir Parking.Infrastructure\Services

# API
mkdir Parking.API\Controllers
mkdir Parking.API\Middleware

# Tests
mkdir Parking.Tests\Handlers
mkdir Parking.Tests\Validators

# Specs
mkdir Parking.Specs\Features
mkdir Parking.Specs\StepDefinitions

# ArchTests
mkdir Parking.ArchTests
```

### Dia 2: Domain Base Classes (3 horas)

**Copie do arquivo `Parking.Domain_BaseClasses.cs` para seus projects:**

1. **Entity.cs** → `Parking.Domain/Common/Entity.cs`
2. **AggregateRoot.cs** → `Parking.Domain/Common/AggregateRoot.cs`
3. **IDomainEvent.cs** → `Parking.Domain/Common/IDomainEvent.cs`
4. **ValueObject.cs** → `Parking.Domain/Common/ValueObject.cs`
5. **Error.cs** → `Parking.Domain/Common/Error.cs`
6. **Result.cs** → `Parking.Domain/Common/Result.cs`

**Compile:**
```bash
dotnet build Parking.Domain
```

---

## 📋 Dia 3-4: Domain Entities + Value Objects (2 dias, 8 horas)

**Copie do arquivo `TEMPLATES_PARTE1_Domain_Infrastructure.cs`:**

### Entities (9 arquivos)

1. **Company.cs** → `Parking.Domain/Entities/Company.cs`
2. **Branch.cs** → `Parking.Domain/Entities/Branch.cs`
3. **Role.cs** → `Parking.Domain/Entities/Role.cs`
4. **Permission.cs** → `Parking.Domain/Entities/Permission.cs`
5. **AppUser.cs** → `Parking.Domain/Entities/AppUser.cs`
6. **RefreshToken.cs** → `Parking.Domain/Entities/RefreshToken.cs`
7. **UserRole.cs** → `Parking.Domain/Entities/UserRole.cs`
8. **RolePermission.cs** → `Parking.Domain/Entities/RolePermission.cs`
9. **AccessLog.cs** → `Parking.Domain/Entities/AccessLog.cs`

### Value Objects (3 arquivos)

1. **Email.cs** → `Parking.Domain/ValueObjects/Email.cs`
2. **Username.cs** → `Parking.Domain/ValueObjects/Username.cs`
3. **PhoneNumber.cs** → `Parking.Domain/ValueObjects/PhoneNumber.cs`

### Repository Interfaces (6 arquivos)

1. **ICompanyRepository.cs** → `Parking.Domain/Repositories/ICompanyRepository.cs`
2. **IBranchRepository.cs** → `Parking.Domain/Repositories/IBranchRepository.cs`
3. **IAppUserRepository.cs** → `Parking.Domain/Repositories/IAppUserRepository.cs`
4. **IRoleRepository.cs** → `Parking.Domain/Repositories/IRoleRepository.cs`
5. **IPermissionRepository.cs** → `Parking.Domain/Repositories/IPermissionRepository.cs`
6. **IUnitOfWork.cs** → `Parking.Domain/Repositories/IUnitOfWork.cs`

**Compile:**
```bash
dotnet build Parking.Domain
```

---

## 📋 Dia 5-7: Infrastructure (3 dias, 12 horas)

### EF Core Setup

1. **AppDbContext.cs** → `Parking.Infrastructure/Persistence/AppDbContext.cs`
2. **CompanyConfiguration.cs** → `Parking.Infrastructure/Persistence/Configurations/CompanyConfiguration.cs`
3-9. Repita para: **Branch, Role, Permission, AppUser, RefreshToken, UserRole, RolePermission, AccessLog**

### Repositories (5 arquivos)

1. **CompanyRepository.cs** → `Parking.Infrastructure/Persistence/Repositories/CompanyRepository.cs`
2-5. Repita para: **Branch, AppUser, Role, Permission**

### Services (3 arquivos)

1. **PasswordHasher.cs** → `Parking.Infrastructure/Services/PasswordHasher.cs`
2. **JwtTokenService.cs** → `Parking.Infrastructure/Services/JwtTokenService.cs`
3. **CurrentUserService.cs** → `Parking.Infrastructure/Services/CurrentUserService.cs`

### DI Setup

1. **DependencyInjection.cs** → `Parking.Infrastructure/DependencyInjection.cs`

**Compile:**
```bash
dotnet build Parking.Infrastructure
```

**Create migrations:**
```bash
dotnet ef migrations add Initial --project Parking.Infrastructure --startup-project Parking.API
```

---

## 📋 Dia 8-9: Application + API (2 dias, 10 horas)

### Application Layer

**Abstractions (6 interfaces):**
- `ICommand.cs`, `ICommandHandler.cs`
- `IQuery.cs`, `IQueryHandler.cs`
- `IPasswordHasher.cs`, `ITokenService.cs`, `ICurrentUser.cs`

**Commands (10 + handlers + validators):**
- `CreateUserCommand` + Handler + Validator
- `LoginCommand` + Handler + Validator
- `RefreshTokenCommand` + Handler + Validator
- `AssignRoleCommand` + Handler + Validator
- `CreateCompanyCommand` + Handler + Validator
- `CreateBranchCommand` + Handler + Validator

**Queries (4 + handlers):**
- `GetAllUsersQuery` + Handler
- `GetAllRolesQuery` + Handler
- `GetCompanyByIdQuery` + Handler
- `GetBranchesByCompanyQuery` + Handler

**DTOs (4 classes):**
- `UserDto.cs`, `RoleDto.cs`, `CompanyDto.cs`, `BranchDto.cs`

**Behaviors (2 classes):**
- `LoggingBehavior.cs`, `ValidationBehavior.cs`

**DependencyInjection.cs**

### API Layer

**Controllers (4 classes):**
- `ApiController.cs` (base)
- `AuthController.cs`
- `CompanyController.cs`
- `BranchController.cs`

**Middleware (1 class):**
- `ExceptionHandlingMiddleware.cs`

**Configuration:**
- `Program.cs` (DI setup + JWT auth + middleware)
- `appsettings.json` (ConnectionString + Jwt settings)
- `appsettings.Development.json`

---

## 📋 Dia 10: Tests (1 dia, 6 horas)

### Unit Tests (4 test classes)
- `CreateUserCommandHandlerTests.cs`
- `LoginCommandHandlerTests.cs`
- `CreateCompanyCommandHandlerTests.cs`
- `GetAllUsersQueryHandlerTests.cs`

**Exemplo de teste:**
```csharp
public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRejectDuplicateEmail()
    {
        // Arrange
        var userRepo = Substitute.For<IAppUserRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        userRepo.ExistsAsync(Arg.Any<string>(), "test@email.com", Arg.Any<CancellationToken>())
            .Returns(true);
        
        var handler = new CreateUserCommandHandler(userRepo, Substitute.For<IPasswordHasher>(), unitOfWork);
        var command = new CreateUserCommand("user", "test@email.com", "John", "Password123");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AppUser.DuplicateEmail");
    }
}
```

### BDD Tests (Reqnroll)
- `Authentication.feature` (4 scenarios)
- `Company.feature` (2 scenarios)
- `AuthenticationSteps.cs`, `CompanySteps.cs`

**Exemplo .feature:**
```gherkin
Feature: User Authentication
  Scenario: Register a new user
    Given no user exists with email "john@example.com"
    When I register with username "john" and password "Password123"
    Then the user should be created
    
  Scenario: Login with valid credentials
    Given a user exists with username "john" and password "Password123"
    When I login with username "john" and password "Password123"
    Then I should receive a JWT token
```

### Architecture Tests (NetArchTest)
- `ArchitectureTests.cs` (10+ rules)

```csharp
public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotReferenceMediatR()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .Should()
            .NotHaveDependencyOn("MediatR")
            .GetResult();

        Assert.True(result.IsSuccessful, result.FailingTypes.First());
    }
}
```

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "CreateUserCommandHandlerTests"

# Run API
dotnet run --project Parking.API

# API will start at https://localhost:7001
```

---

## ✅ End of Fase 1 Checklist

- [ ] All 7 projects created and referencing correctly
- [ ] Domain base classes: Entity, AggregateRoot, ValueObject, Error, Result
- [ ] 9 domain entities implemented
- [ ] 3 value objects implemented
- [ ] 6 repository interfaces defined
- [ ] AppDbContext + 9 EF configurations
- [ ] 5 repositories implemented
- [ ] 3 services (PasswordHasher, JwtTokenService, CurrentUserService)
- [ ] 10 commands + handlers + validators
- [ ] 4 queries + handlers
- [ ] 4 DTOs
- [ ] 2 pipeline behaviors
- [ ] ApiController + 4 controllers
- [ ] ExceptionHandlingMiddleware
- [ ] Program.cs DI setup + JWT auth
- [ ] Unit tests (4 test classes)
- [ ] BDD tests (Reqnroll features + steps)
- [ ] Architecture tests
- [ ] All tests passing
- [ ] API runs without errors
- [ ] appsettings.json configured

---

## 🚀 Success = All This Works

```bash
# 1. Build
dotnet build

# 2. Create database
dotnet ef database update --project Parking.Infrastructure --startup-project Parking.API

# 3. Run API
dotnet run --project Parking.API

# 4. Test endpoints
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","email":"test@example.com","fullName":"Test User","password":"Password123"}'

# 5. Run tests
dotnet test
```

---

**Next Phase:** 🟠 Fase 2 — Operacional (Employees + Cash + Parking Spaces)

**Expected outcome:** Auth fully working, clean architecture validated, all tests passing
