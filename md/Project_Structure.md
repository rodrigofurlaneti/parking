# Parking Project Structure — .NET 9 + Clean Architecture

## Criar Solução e Projects

```bash
cd backend

# Criar solução
dotnet new sln -n Parking

# Criar class libraries
dotnet new classlib -n Parking.Domain
dotnet new classlib -n Parking.Application
dotnet new classlib -n Parking.Infrastructure
dotnet new webapi -n Parking.API
dotnet new xunit -n Parking.Tests
dotnet new classlib -n Parking.Specs
dotnet new classlib -n Parking.ArchTests

# Adicionar projects à solução
dotnet sln add Parking.Domain
dotnet sln add Parking.Application
dotnet sln add Parking.Infrastructure
dotnet sln add Parking.API
dotnet sln add Parking.Tests
dotnet sln add Parking.Specs
dotnet sln add Parking.ArchTests

# Criar referências
cd Parking.Application && dotnet add reference ../Parking.Domain && cd ..
cd Parking.Infrastructure && dotnet add reference ../Parking.Domain && cd ..
cd Parking.API && dotnet add reference ../Parking.Application && dotnet add reference ../Parking.Infrastructure && cd ..
cd Parking.Tests && dotnet add reference ../Parking.Domain && dotnet add reference ../Parking.Application && dotnet add reference ../Parking.Infrastructure && cd ..
cd Parking.Specs && dotnet add reference ../Parking.Application && cd ..
cd Parking.ArchTests && dotnet add reference ../Parking.Domain && dotnet add reference ../Parking.Application && dotnet add reference ../Parking.Infrastructure && dotnet add reference ../Parking.API && cd ..
```

## Instalar NuGets (por project)

### Parking.Domain
```bash
cd Parking.Domain
dotnet add package MediatR
cd ..
```

### Parking.Application
```bash
cd Parking.Application
dotnet add package MediatR
dotnet add package FluentValidation
cd ..
```

### Parking.Infrastructure
```bash
cd Parking.Infrastructure
dotnet add package EntityFramework.NamingConventions
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.5
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next
cd ..
```

### Parking.API
```bash
cd Parking.API
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
cd ..
```

### Parking.Tests
```bash
cd Parking.Tests
dotnet add package xUnit
dotnet add package FluentAssertions
dotnet add package NSubstitute
dotnet add package Moq
cd ..
```

### Parking.Specs
```bash
cd Parking.Specs
dotnet add package Reqnroll
dotnet add package SpecFlow.Plus.LightBDD
cd ..
```

### Parking.ArchTests
```bash
cd Parking.ArchTests
dotnet add package NetArchTest.Rules
cd ..
```

## Estrutura de Pastas

```
Parking/
└── backend/
    ├── Parking.sln
    ├── Parking.Domain/
    │   ├── Parking.Domain.csproj
    │   ├── Common/
    │   │   ├── Entity.cs
    │   │   ├── AggregateRoot.cs
    │   │   ├── ValueObject.cs
    │   │   ├── Error.cs
    │   │   ├── Result.cs
    │   │   └── IDomainEvent.cs
    │   ├── Entities/
    │   │   ├── Company.cs
    │   │   ├── Branch.cs
    │   │   ├── Role.cs
    │   │   ├── Permission.cs
    │   │   ├── AppUser.cs
    │   │   ├── RefreshToken.cs
    │   │   ├── UserRole.cs
    │   │   ├── RolePermission.cs
    │   │   └── AccessLog.cs
    │   ├── ValueObjects/
    │   │   ├── Email.cs
    │   │   ├── Username.cs
    │   │   └── PhoneNumber.cs
    │   └── Repositories/
    │       ├── ICompanyRepository.cs
    │       ├── IBranchRepository.cs
    │       ├── IAppUserRepository.cs
    │       ├── IRoleRepository.cs
    │       ├── IPermissionRepository.cs
    │       └── IUnitOfWork.cs
    │
    ├── Parking.Application/
    │   ├── Parking.Application.csproj
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
    │   ├── Parking.Infrastructure.csproj
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs
    │   │   ├── Configurations/
    │   │   │   ├── CompanyConfiguration.cs
    │   │   │   ├── BranchConfiguration.cs
    │   │   │   ├── AppUserConfiguration.cs
    │   │   │   ├── RoleConfiguration.cs
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
    │   ├── Parking.API.csproj
    │   ├── Controllers/
    │   │   ├── ApiController.cs
    │   │   ├── AuthController.cs
    │   │   ├── CompanyController.cs
    │   │   └── BranchController.cs
    │   ├── Middleware/
    │   │   └── ExceptionHandlingMiddleware.cs
    │   ├── appsettings.json
    │   ├── appsettings.Development.json
    │   ├── Program.cs
    │   ├── Properties/
    │   │   └── launchSettings.json
    │   └── Dockerfile (opcional)
    │
    ├── Parking.Tests/
    │   ├── Parking.Tests.csproj
    │   ├── Handlers/
    │   │   ├── CreateUserCommandHandlerTests.cs
    │   │   ├── LoginCommandHandlerTests.cs
    │   │   └── CreateCompanyCommandHandlerTests.cs
    │   └── Validators/
    │       └── CreateUserCommandValidatorTests.cs
    │
    ├── Parking.Specs/
    │   ├── Parking.Specs.csproj
    │   ├── Features/
    │   │   ├── Authentication.feature
    │   │   └── Company.feature
    │   └── StepDefinitions/
    │       ├── AuthenticationSteps.cs
    │       └── CompanySteps.cs
    │
    └── Parking.ArchTests/
        ├── Parking.ArchTests.csproj
        └── ArchitectureTests.cs
```

## Build & Run

```bash
# Build
dotnet build

# Run API
cd Parking.API
dotnet run

# Run Tests
cd ../Parking.Tests
dotnet test

# Run Arch Tests
cd ../Parking.ArchTests
dotnet test
```

---

**Status:** Ready for implementation  
**Próximo Passo:** Implementar Domain layer (base classes + entities)
