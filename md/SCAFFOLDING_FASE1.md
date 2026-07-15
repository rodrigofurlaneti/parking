# Fase 1 — Scaffolding Completo

Execute este guia passo-a-passo para gerar toda a estrutura .NET 9 da Fase 1.

## 1️⃣ Setup Inicial (30 min)

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

# Adicionar projects à solução
dotnet sln add Parking.Domain
dotnet sln add Parking.Application
dotnet sln add Parking.Infrastructure
dotnet sln add Parking.API
dotnet sln add Parking.Tests
dotnet sln add Parking.Specs
dotnet sln add Parking.ArchTests
```

## 2️⃣ Configurar Referências Entre Projects

```bash
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
```

## 3️⃣ Instalar NuGets

```bash
# Parking.Domain
cd Parking.Domain
dotnet add package MediatR
cd ..

# Parking.Application
cd Parking.Application
dotnet add package MediatR
dotnet add package FluentValidation
cd ..

# Parking.Infrastructure
cd Parking.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.5
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

## 4️⃣ Criar Estrutura de Pastas

```bash
# Domain
mkdir Parking.Domain/Common
mkdir Parking.Domain/Entities
mkdir Parking.Domain/ValueObjects
mkdir Parking.Domain/Repositories

# Application
mkdir Parking.Application/Abstractions
mkdir Parking.Application/Abstractions/Messaging
mkdir Parking.Application/Abstractions/Services
mkdir Parking.Application/Features
mkdir Parking.Application/Features/Auth
mkdir Parking.Application/Features/Auth/CreateUser
mkdir Parking.Application/Features/Auth/Login
mkdir Parking.Application/Features/Auth/RefreshToken
mkdir Parking.Application/Features/Auth/AssignRole
mkdir Parking.Application/Features/Auth/GetUsers
mkdir Parking.Application/Features/Company
mkdir Parking.Application/Features/Company/Create
mkdir Parking.Application/Features/Company/GetById
mkdir Parking.Application/Features/Branch
mkdir Parking.Application/Features/Branch/Create
mkdir Parking.Application/Features/Branch/GetById
mkdir Parking.Application/Features/Common
mkdir Parking.Application/Features/Common/DTOs
mkdir Parking.Application/Behaviors

# Infrastructure
mkdir Parking.Infrastructure/Persistence
mkdir Parking.Infrastructure/Persistence/Configurations
mkdir Parking.Infrastructure/Persistence/Repositories
mkdir Parking.Infrastructure/Services

# API
mkdir Parking.API/Controllers
mkdir Parking.API/Middleware

# Tests
mkdir Parking.Tests/Handlers
mkdir Parking.Tests/Validators

# Specs
mkdir Parking.Specs/Features
mkdir Parking.Specs/StepDefinitions

# ArchTests
mkdir Parking.ArchTests
```

## 5️⃣ Criar Arquivos Base

### Domain/Common

**Entity.cs**
```csharp
namespace Parking.Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity() { }
    protected Entity(long id) => Id = id;

    public bool Equals(Entity? other) => other is not null && Id == other.Id && GetType() == other.GetType();
    public override bool Equals(object? obj) => obj is Entity entity && Equals(entity);
    public override int GetHashCode() => Id.GetHashCode();
    public static bool operator ==(Entity? left, Entity? right) => left?.Equals(right) ?? right is null;
    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}
```

**AggregateRoot.cs**
```csharp
namespace Parking.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot() { }
    protected AggregateRoot(long id) : base(id) { }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
```

**IDomainEvent.cs**
```csharp
namespace Parking.Domain.Common;

public interface IDomainEvent { }
```

**ValueObject.cs**
```csharp
namespace Parking.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetAtomicValues();

    public bool Equals(ValueObject? other) => 
        other is not null && GetAtomicValues().SequenceEqual(other.GetAtomicValues());

    public override bool Equals(object? obj) => obj is ValueObject vo && Equals(vo);

    public override int GetHashCode() =>
        GetAtomicValues()
            .Aggregate(default(HashCode), (hashCode, value) =>
            {
                hashCode.Add(value);
                return hashCode;
            })
            .ToHashCode();

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !(left == right);
}
```

**Error.cs**
```csharp
namespace Parking.Domain.Common;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
```

**Result.cs**
```csharp
namespace Parking.Domain.Common;

public abstract record Result
{
    public sealed record Success : Result;
    public sealed record Failure(Error Error) : Result;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result Success() => new Success();
    public static Result Failure(Error error) => new Failure(error);
}

public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(Error Error) : Result<T>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result<T> Success(T value) => new Success(value);
    public static Result<T> Failure(Error error) => new Failure(error);
}
```

## 📋 Próximos Passos

**Após rodar este scaffolding:**

1. ✅ Copie os arquivos base acima para Parking.Domain/Common/
2. ⏳ Implemente Domain/Entities (Company, Branch, Role, AppUser, etc)
3. ⏳ Implemente Domain/ValueObjects (Email, Username, PhoneNumber)
4. ⏳ Implemente Domain/Repositories (interfaces)
5. ⏳ Implemente Infrastructure/Persistence/AppDbContext
6. ⏳ Implemente Infrastructure/Persistence/Configurations (9 EF configs)
7. ⏳ Implemente Infrastructure/Persistence/Repositories (5 implementations)
8. ⏳ Implemente Infrastructure/Services (3 services)
9. ⏳ Implemente Application/Features (10 commands + 4 queries)
10. ⏳ Implemente API/Controllers (4 controllers)
11. ⏳ Implemente Testes (xUnit + Reqnroll + NetArchTest)

## ✅ Build & Test

```bash
# Build solução
dotnet build

# Run API
dotnet run --project Parking.API

# Run Testes
dotnet test
```

---

**Status:** Scaffolding structure created  
**Next:** Implement Domain layer entities from templates in Parking.Domain_BaseClasses.cs
