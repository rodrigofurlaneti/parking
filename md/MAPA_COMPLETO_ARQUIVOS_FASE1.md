# 📁 Mapa Completo de Arquivos — Fase 1

Todos os 50+ arquivos que você precisa criar, com caminho exato e status.

---

## 🗂️ Estrutura Completa

```
backend/
├── Parking.sln (crie com: dotnet new sln)
│
├── Parking.Domain/                                    [ZERO external dependencies]
│   ├── Parking.Domain.csproj (template: classlib)
│   ├── Common/
│   │   ├── Entity.cs                                 ← SCAFFOLDING_FASE1.md seção 5
│   │   ├── AggregateRoot.cs                          ← SCAFFOLDING_FASE1.md seção 5
│   │   ├── IDomainEvent.cs                           ← SCAFFOLDING_FASE1.md seção 5
│   │   ├── ValueObject.cs                            ← SCAFFOLDING_FASE1.md seção 5
│   │   ├── Error.cs                                  ← SCAFFOLDING_FASE1.md seção 5
│   │   └── Result.cs                                 ← SCAFFOLDING_FASE1.md seção 5
│   │
│   ├── Entities/
│   │   ├── Company.cs                                ← TEMPLATES_PARTE1
│   │   ├── Branch.cs                                 ← TEMPLATES_PARTE1
│   │   ├── Role.cs                                   ← TEMPLATES_PARTE1
│   │   ├── Permission.cs                             ← TEMPLATES_PARTE1
│   │   ├── AppUser.cs                                ← TEMPLATES_PARTE1
│   │   ├── RefreshToken.cs                           ← TEMPLATES_PARTE1
│   │   ├── UserRole.cs                               ← TEMPLATES_PARTE1
│   │   ├── RolePermission.cs                         ← TEMPLATES_PARTE1
│   │   └── AccessLog.cs                              ← TEMPLATES_PARTE1
│   │
│   ├── ValueObjects/
│   │   ├── Email.cs                                  ← TEMPLATES_PARTE1
│   │   ├── Username.cs                               ← TEMPLATES_PARTE1
│   │   └── PhoneNumber.cs                            ← TEMPLATES_PARTE1
│   │
│   └── Repositories/
│       ├── ICompanyRepository.cs                     ← TEMPLATES_PARTE1
│       ├── IBranchRepository.cs                      ← TEMPLATES_PARTE1
│       ├── IAppUserRepository.cs                     ← TEMPLATES_PARTE1
│       ├── IRoleRepository.cs                        ← TEMPLATES_PARTE1
│       ├── IPermissionRepository.cs                  ← TEMPLATES_PARTE1
│       └── IUnitOfWork.cs                            ← TEMPLATES_PARTE1
│
├── Parking.Application/                              [Depends on: Domain]
│   ├── Parking.Application.csproj (template: classlib)
│   ├── Abstractions/
│   │   ├── Messaging/
│   │   │   ├── ICommand.cs                           ← TEMPLATES_PARTE2
│   │   │   ├── ICommandHandler.cs                    ← TEMPLATES_PARTE2
│   │   │   ├── IQuery.cs                             ← TEMPLATES_PARTE2
│   │   │   └── IQueryHandler.cs                      ← TEMPLATES_PARTE2
│   │   │
│   │   └── Services/
│   │       ├── IPasswordHasher.cs                    ← TEMPLATES_PARTE2
│   │       ├── ITokenService.cs                      ← TEMPLATES_PARTE2
│   │       └── ICurrentUser.cs                       ← TEMPLATES_PARTE2
│   │
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── CreateUser/
│   │   │   │   ├── CreateUserCommand.cs              ← TEMPLATES_PARTE2
│   │   │   │   ├── CreateUserCommandHandler.cs       ← TEMPLATES_PARTE2
│   │   │   │   └── CreateUserCommandValidator.cs     ← TEMPLATES_PARTE2
│   │   │   │
│   │   │   ├── Login/
│   │   │   │   ├── LoginCommand.cs                   ← TEMPLATES_PARTE2
│   │   │   │   ├── LoginCommandHandler.cs            ← TEMPLATES_PARTE2
│   │   │   │   └── LoginCommandValidator.cs          ← TEMPLATES_PARTE2
│   │   │   │
│   │   │   ├── RefreshToken/
│   │   │   │   ├── RefreshTokenCommand.cs            ← TEMPLATES_PARTE2
│   │   │   │   ├── RefreshTokenCommandHandler.cs     ← TEMPLATES_PARTE2
│   │   │   │   └── RefreshTokenCommandValidator.cs   ← TEMPLATES_PARTE2
│   │   │   │
│   │   │   ├── AssignRole/
│   │   │   │   ├── AssignRoleCommand.cs              ← TEMPLATES_PARTE2
│   │   │   │   ├── AssignRoleCommandHandler.cs       ← TEMPLATES_PARTE2
│   │   │   │   └── AssignRoleCommandValidator.cs     ← TEMPLATES_PARTE2
│   │   │   │
│   │   │   └── GetUsers/
│   │   │       ├── GetAllUsersQuery.cs               ← TEMPLATES_PARTE2
│   │   │       └── GetAllUsersQueryHandler.cs        ← TEMPLATES_PARTE2
│   │   │
│   │   ├── Company/
│   │   │   ├── Create/
│   │   │   │   ├── CreateCompanyCommand.cs           ← TEMPLATES_PARTE2
│   │   │   │   ├── CreateCompanyCommandHandler.cs    ← TEMPLATES_PARTE2
│   │   │   │   └── CreateCompanyCommandValidator.cs  ← TEMPLATES_PARTE2
│   │   │   │
│   │   │   └── GetById/
│   │   │       ├── GetCompanyByIdQuery.cs            ← TEMPLATES_PARTE2
│   │   │       └── GetCompanyByIdQueryHandler.cs     ← TEMPLATES_PARTE2
│   │   │
│   │   ├── Branch/
│   │   │   ├── Create/
│   │   │   │   ├── CreateBranchCommand.cs            ← TEMPLATES_PARTE2
│   │   │   │   ├── CreateBranchCommandHandler.cs     ← TEMPLATES_PARTE2
│   │   │   │   └── CreateBranchCommandValidator.cs   ← TEMPLATES_PARTE2
│   │   │   │
│   │   │   └── GetByCompany/
│   │   │       ├── GetBranchesByCompanyQuery.cs      ← TEMPLATES_PARTE2
│   │   │       └── GetBranchesByCompanyQueryHandler.cs ← TEMPLATES_PARTE2
│   │   │
│   │   └── Common/
│   │       └── DTOs/
│   │           ├── UserDto.cs                        ← TEMPLATES_PARTE2
│   │           ├── RoleDto.cs                        ← TEMPLATES_PARTE2
│   │           ├── CompanyDto.cs                     ← TEMPLATES_PARTE2
│   │           └── BranchDto.cs                      ← TEMPLATES_PARTE2
│   │
│   ├── Behaviors/
│   │   ├── LoggingBehavior.cs                        ← TEMPLATES_PARTE2
│   │   └── ValidationBehavior.cs                     ← TEMPLATES_PARTE2
│   │
│   └── DependencyInjection.cs                        ← TEMPLATES_PARTE2
│
├── Parking.Infrastructure/                           [Depends on: Domain]
│   ├── Parking.Infrastructure.csproj (template: classlib)
│   ├── Persistence/
│   │   ├── AppDbContext.cs                           ← TEMPLATES_PARTE1
│   │   │
│   │   ├── Configurations/
│   │   │   ├── CompanyConfiguration.cs               ← TEMPLATES_PARTE1
│   │   │   ├── BranchConfiguration.cs                ← TEMPLATES_PARTE1
│   │   │   ├── RoleConfiguration.cs                  ← TEMPLATES_PARTE1
│   │   │   ├── PermissionConfiguration.cs            ← TEMPLATES_PARTE1
│   │   │   ├── AppUserConfiguration.cs               ← TEMPLATES_PARTE1
│   │   │   ├── RefreshTokenConfiguration.cs          ← TEMPLATES_PARTE1
│   │   │   ├── UserRoleConfiguration.cs              ← TEMPLATES_PARTE1
│   │   │   ├── RolePermissionConfiguration.cs        ← TEMPLATES_PARTE1
│   │   │   └── AccessLogConfiguration.cs             ← TEMPLATES_PARTE1
│   │   │
│   │   └── Repositories/
│   │       ├── CompanyRepository.cs                  ← TEMPLATES_PARTE1
│   │       ├── BranchRepository.cs                   ← TEMPLATES_PARTE1
│   │       ├── AppUserRepository.cs                  ← TEMPLATES_PARTE1
│   │       ├── RoleRepository.cs                     ← TEMPLATES_PARTE1
│   │       └── PermissionRepository.cs               ← TEMPLATES_PARTE1
│   │
│   ├── Services/
│   │   ├── PasswordHasher.cs                         ← TEMPLATES_PARTE1
│   │   ├── JwtTokenService.cs                        ← TEMPLATES_PARTE1
│   │   └── CurrentUserService.cs                     ← TEMPLATES_PARTE1
│   │
│   └── DependencyInjection.cs                        ← TEMPLATES_PARTE1
│
├── Parking.API/                                      [Depends on: Application, Infrastructure]
│   ├── Parking.API.csproj (template: webapi)
│   ├── Controllers/
│   │   ├── ApiController.cs                          ← TEMPLATES_PARTE2
│   │   ├── AuthController.cs                         ← TEMPLATES_PARTE2
│   │   ├── CompanyController.cs                      ← TEMPLATES_PARTE2
│   │   └── BranchController.cs                       ← TEMPLATES_PARTE2
│   │
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs            ← TEMPLATES_PARTE2
│   │
│   ├── Program.cs                                    ← TEMPLATES_Program_Appsettings.cs
│   ├── appsettings.json                              ← TEMPLATES_Program_Appsettings.cs
│   ├── appsettings.Development.json                  ← TEMPLATES_Program_Appsettings.cs
│   └── Properties/
│       └── launchSettings.json (auto-gerado)
│
├── Parking.Tests/                                    [Depends on: Application, Infrastructure]
│   ├── Parking.Tests.csproj (template: xunit)
│   ├── Handlers/
│   │   ├── CreateUserCommandHandlerTests.cs          ← TEMPLATES_PARTE2
│   │   ├── LoginCommandHandlerTests.cs               ← TEMPLATES_PARTE2
│   │   ├── CreateCompanyCommandHandlerTests.cs       ← TEMPLATES_PARTE2
│   │   └── GetAllUsersQueryHandlerTests.cs           ← TEMPLATES_PARTE2
│   │
│   └── Validators/
│       └── (opcional — adicione conforme necessário)
│
├── Parking.Specs/                                    [Depends on: Application]
│   ├── Parking.Specs.csproj (template: classlib)
│   ├── Features/
│   │   ├── Authentication.feature                    ← TEMPLATES_PARTE2
│   │   └── Company.feature                           ← TEMPLATES_PARTE2
│   │
│   └── StepDefinitions/
│       ├── AuthenticationSteps.cs                    ← TEMPLATES_PARTE2
│       └── CompanySteps.cs                           ← TEMPLATES_PARTE2
│
└── Parking.ArchTests/                                [Depends on: All four]
    ├── Parking.ArchTests.csproj (template: classlib)
    └── ArchitectureTests.cs                          ← TEMPLATES_PARTE2
```

---

## 📊 Contagem Total

| Camada | Tipo | Qtd | Arquivo Template |
|--------|------|-----|------------------|
| Domain | Base Classes | 6 | SCAFFOLDING_FASE1.md |
| Domain | Entities | 9 | TEMPLATES_PARTE1 |
| Domain | Value Objects | 3 | TEMPLATES_PARTE1 |
| Domain | Repositories | 6 | TEMPLATES_PARTE1 |
| Infrastructure | EF Context | 1 | TEMPLATES_PARTE1 |
| Infrastructure | Configurations | 9 | TEMPLATES_PARTE1 |
| Infrastructure | Repositories | 5 | TEMPLATES_PARTE1 |
| Infrastructure | Services | 3 | TEMPLATES_PARTE1 |
| Infrastructure | DI | 1 | TEMPLATES_PARTE1 |
| Application | Abstractions (Messaging) | 4 | TEMPLATES_PARTE2 |
| Application | Abstractions (Services) | 3 | TEMPLATES_PARTE2 |
| Application | Commands | 6 | TEMPLATES_PARTE2 |
| Application | Handlers | 6 | TEMPLATES_PARTE2 |
| Application | Validators | 6 | TEMPLATES_PARTE2 |
| Application | Queries | 4 | TEMPLATES_PARTE2 |
| Application | Query Handlers | 4 | TEMPLATES_PARTE2 |
| Application | DTOs | 4 | TEMPLATES_PARTE2 |
| Application | Behaviors | 2 | TEMPLATES_PARTE2 |
| Application | DI | 1 | TEMPLATES_PARTE2 |
| API | Controllers | 4 | TEMPLATES_PARTE2 |
| API | Middleware | 1 | TEMPLATES_PARTE2 |
| API | Config | 3 | TEMPLATES_Program_Appsettings.cs |
| Tests | Unit Tests | 4 | TEMPLATES_PARTE2 |
| Tests | BDD Features | 2 | TEMPLATES_PARTE2 |
| Tests | BDD Steps | 2 | TEMPLATES_PARTE2 |
| Tests | Architecture | 1 | TEMPLATES_PARTE2 |
| **TOTAL** | | **116** | |

---

## 🔄 Ordem de Criação Recomendada

```
1. Projects + referências (Pasta.csproj)
2. Pastas (mkdir)
3. Domain base classes (6 arquivos)
4. Domain entities + VOs + repos (18 arquivos)
5. Infrastructure services + DI (14 arquivos)
6. Application abstractions + DTOs (11 arquivos)
7. Application commands/queries (20 arquivos)
8. API controllers + middleware + config (8 arquivos)
9. Tests (9 arquivos)
```

---

## 🎯 Status Tracking

Use este checkbox para acompanhar seu progresso:

### Domain Layer (27 arquivos)
- [ ] Entity.cs
- [ ] AggregateRoot.cs
- [ ] IDomainEvent.cs
- [ ] ValueObject.cs
- [ ] Error.cs
- [ ] Result.cs
- [ ] Company.cs
- [ ] Branch.cs
- [ ] Role.cs
- [ ] Permission.cs
- [ ] AppUser.cs
- [ ] RefreshToken.cs
- [ ] UserRole.cs
- [ ] RolePermission.cs
- [ ] AccessLog.cs
- [ ] Email.cs
- [ ] Username.cs
- [ ] PhoneNumber.cs
- [ ] ICompanyRepository.cs
- [ ] IBranchRepository.cs
- [ ] IAppUserRepository.cs
- [ ] IRoleRepository.cs
- [ ] IPermissionRepository.cs
- [ ] IUnitOfWork.cs

### Infrastructure Layer (18 arquivos)
- [ ] AppDbContext.cs
- [ ] CompanyConfiguration.cs
- [ ] BranchConfiguration.cs
- [ ] RoleConfiguration.cs
- [ ] PermissionConfiguration.cs
- [ ] AppUserConfiguration.cs
- [ ] RefreshTokenConfiguration.cs
- [ ] UserRoleConfiguration.cs
- [ ] RolePermissionConfiguration.cs
- [ ] AccessLogConfiguration.cs
- [ ] CompanyRepository.cs
- [ ] BranchRepository.cs
- [ ] AppUserRepository.cs
- [ ] RoleRepository.cs
- [ ] PermissionRepository.cs
- [ ] PasswordHasher.cs
- [ ] JwtTokenService.cs
- [ ] CurrentUserService.cs
- [ ] DependencyInjection.cs (Infrastructure)

### Application Layer (31 arquivos)
- [ ] ICommand.cs
- [ ] ICommandHandler.cs
- [ ] IQuery.cs
- [ ] IQueryHandler.cs
- [ ] IPasswordHasher.cs
- [ ] ITokenService.cs
- [ ] ICurrentUser.cs
- [ ] UserDto.cs
- [ ] RoleDto.cs
- [ ] CompanyDto.cs
- [ ] BranchDto.cs
- [ ] CreateUserCommand.cs
- [ ] CreateUserCommandHandler.cs
- [ ] CreateUserCommandValidator.cs
- [ ] LoginCommand.cs
- [ ] LoginCommandHandler.cs
- [ ] LoginCommandValidator.cs
- [ ] RefreshTokenCommand.cs
- [ ] RefreshTokenCommandHandler.cs
- [ ] RefreshTokenCommandValidator.cs
- [ ] AssignRoleCommand.cs
- [ ] AssignRoleCommandHandler.cs
- [ ] AssignRoleCommandValidator.cs
- [ ] GetAllUsersQuery.cs
- [ ] GetAllUsersQueryHandler.cs
- [ ] CreateCompanyCommand.cs
- [ ] CreateCompanyCommandHandler.cs
- [ ] CreateCompanyCommandValidator.cs
- [ ] GetCompanyByIdQuery.cs
- [ ] GetCompanyByIdQueryHandler.cs
- [ ] CreateBranchCommand.cs
- [ ] CreateBranchCommandHandler.cs
- [ ] CreateBranchCommandValidator.cs
- [ ] GetBranchesByCompanyQuery.cs
- [ ] GetBranchesByCompanyQueryHandler.cs
- [ ] LoggingBehavior.cs
- [ ] ValidationBehavior.cs
- [ ] DependencyInjection.cs (Application)

### API Layer (8 arquivos)
- [ ] ApiController.cs
- [ ] AuthController.cs
- [ ] CompanyController.cs
- [ ] BranchController.cs
- [ ] ExceptionHandlingMiddleware.cs
- [ ] Program.cs
- [ ] appsettings.json
- [ ] appsettings.Development.json

### Tests (9 arquivos)
- [ ] CreateUserCommandHandlerTests.cs
- [ ] LoginCommandHandlerTests.cs
- [ ] CreateCompanyCommandHandlerTests.cs
- [ ] GetAllUsersQueryHandlerTests.cs
- [ ] Authentication.feature
- [ ] Company.feature
- [ ] AuthenticationSteps.cs
- [ ] CompanySteps.cs
- [ ] ArchitectureTests.cs

---

## 💡 Dica

Imprima este arquivo ou salve em seu favorito. Ao criar cada arquivo, marque o checkbox.

**Quando terminar todos os 116 arquivos → Você tem Fase 1 completo! ✨**

