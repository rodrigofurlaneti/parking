# Parking System Backend - Quick Start Guide

## 🚀 Getting Started in 5 Minutes

### Step 1: Open the Solution
```bash
cd C:\Users\AMD\Documents\Claude\Projects\Parking\backend\src
```

Open `Parking.sln` in Visual Studio 2022

### Step 2: Restore Packages
```bash
dotnet restore
```

### Step 3: Build
```bash
dotnet build
```

### Step 4: Run Tests
```bash
dotnet test
```

### Step 5: Start API
```bash
cd Parking.API
dotnet run
```

API will be available at: `https://localhost:7000`

---

## 📁 Project Structure at a Glance

```
7 Projects organized by layer:

Domain Layer (Core Business Logic)
└── Parking.Domain
    - Contains: Entities, ValueObjects, Interfaces

Application Layer (Commands & Queries)
└── Parking.Application
    - Contains: DTOs, Validators, Commands, Queries, Handlers

Infrastructure Layer (Data Access)
└── Parking.Infrastructure
    - Contains: DbContext, Repositories, Migrations

API Layer (REST Endpoints)
└── Parking.API
    - Contains: Controllers, Middleware, Configuration

Testing Layers
├── Parking.Tests (Unit & Integration)
├── Parking.Specs (BDD with SpecFlow)
└── Parking.ArchTests (Architecture Validation)
```

---

## 🔧 Common Commands

### Build & Test
```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run specific project tests
dotnet test Parking.Tests

# Run with verbose output
dotnet test --verbosity detailed
```

### Database
```bash
# Add migration
dotnet ef migrations add MigrationName -p Parking.Infrastructure

# Update database
dotnet ef database update -p Parking.Infrastructure

# List migrations
dotnet ef migrations list -p Parking.Infrastructure
```

### API
```bash
# Run API
dotnet run -p Parking.API

# Run with custom port
dotnet run -p Parking.API -- --urls="https://localhost:7001"
```

### IDE
```bash
# Open in VS Code
code .

# Format code
dotnet format

# Analyze code
dotnet analyzers
```

---

## 📋 Project Templates

### Add a Command Handler

Location: `Parking.Application/Commands/YourCommand/`

```csharp
// CreateYourEntityCommand.cs
public record CreateYourEntityCommand(string Name) : IRequest<YourEntityDto>;

// CreateYourEntityCommandHandler.cs
public class CreateYourEntityCommandHandler : IRequestHandler<CreateYourEntityCommand, YourEntityDto>
{
    public async Task<YourEntityDto> Handle(CreateYourEntityCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Add a Query Handler

Location: `Parking.Application/Queries/YourQuery/`

```csharp
// GetYourEntitiesQuery.cs
public record GetYourEntitiesQuery : IRequest<List<YourEntityDto>>;

// GetYourEntitiesQueryHandler.cs
public class GetYourEntitiesQueryHandler : IRequestHandler<GetYourEntitiesQuery, List<YourEntityDto>>
{
    public async Task<List<YourEntityDto>> Handle(GetYourEntitiesQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Add a Controller

Location: `Parking.API/Controllers/`

```csharp
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    private readonly IMediator _mediator;

    public YourController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<ActionResult<YourEntityDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetYourEntityQuery(id));
        return Ok(result);
    }
}
```

---

## 🧪 Testing

### Unit Test Example
Location: `Parking.Tests/Unit/`

```csharp
public class YourServiceTests
{
    [Fact]
    public async Task Handle_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateYourEntityCommand("Test");
        var handler = new CreateYourEntityCommandHandler();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Handle_WithInvalidInput_ThrowsException(string invalidName)
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            handler.Handle(new CreateYourEntityCommand(invalidName), CancellationToken.None));
    }
}
```

### BDD Feature Example
Location: `Parking.Specs/Features/`

```gherkin
Feature: Manage Parking Spaces
    As a parking manager
    I want to manage parking spaces
    So that I can track availability

    Scenario: Create a new parking space
        Given the system has no parking spaces
        When I create a parking space with name "A1"
        Then the parking space "A1" should exist
        And the total count should be 1

    Scenario: Delete a parking space
        Given I have a parking space "A1"
        When I delete the parking space "A1"
        Then the parking space "A1" should not exist
```

---

## 🗄️ Database Setup

### Connection String
**Development**: 
```
Server=(localdb)\mssqllocaldb;Database=ParkingDb_Dev;Trusted_Connection=true;
```

**Production**: 
```
Server=(localdb)\mssqllocaldb;Database=ParkingDb;Trusted_Connection=true;
```

### Create DbContext

Location: `Parking.Infrastructure/Persistence/`

```csharp
public class ParkingDbContext : DbContext
{
    public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options) { }

    public DbSet<ParkingSpace> ParkingSpaces { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ParkingDbContext).Assembly);
    }
}
```

### Entity Configuration Example

Location: `Parking.Infrastructure/Persistence/Configurations/`

```csharp
public class ParkingSpaceConfiguration : IEntityTypeConfiguration<ParkingSpace>
{
    public void Configure(EntityTypeBuilder<ParkingSpace> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.Property(p => p.IsAvailable).HasDefaultValue(true);
    }
}
```

---

## 🔌 API Endpoints

### Health Check
```
GET /api/health

Response:
{
  "status": "API is running",
  "timestamp": "2026-07-15T10:00:00Z"
}
```

### Swagger Documentation
```
https://localhost:7000/swagger
```

---

## 📊 Logging

All logs are configured via Serilog:

### Development
- **Output**: Console + File (`logs/parking-api-*.txt`)
- **Level**: Debug

### Production
- **Output**: File only
- **Level**: Information
- **Rolling**: Daily intervals

### Usage in Code
```csharp
private readonly ILogger<YourClass> _logger;

public YourClass(ILogger<YourClass> logger)
{
    _logger = logger;
}

public void SomeMethod()
{
    _logger.LogInformation("Starting operation");
    _logger.LogWarning("Warning message");
    _logger.LogError("Error message");
}
```

---

## 🔍 Architecture Validation

Run architecture tests:
```bash
dotnet test Parking.ArchTests
```

This validates:
- Layering rules are enforced
- Dependencies are correct
- Naming conventions are followed

---

## 📦 Adding NuGet Packages

```bash
# Add to a project
dotnet add Parking.Domain package [PackageName] --version [Version]

# Example:
dotnet add Parking.Domain package MediatR --version 12.3.0
```

---

## 🐛 Debugging

### Visual Studio
1. Set breakpoints (F9)
2. Start debugging (F5)
3. Use Debug > Windows > Immediate (Ctrl+Alt+I)

### Command Line
```bash
# Run with debugging
dotnet run --configuration Debug

# Run tests in debug mode
dotnet test --configuration Debug --logger "console;verbosity=detailed"
```

---

## 📚 File Locations

| Purpose | Location |
|---------|----------|
| Commands | `Parking.Application/Commands/` |
| Queries | `Parking.Application/Queries/` |
| Handlers | `Parking.Application/Handlers/` |
| Entities | `Parking.Domain/Entities/` |
| Controllers | `Parking.API/Controllers/` |
| Tests | `Parking.Tests/` |
| Migrations | `Parking.Infrastructure/Persistence/Migrations/` |
| DB Context | `Parking.Infrastructure/Persistence/` |

---

## ⚡ Performance Tips

1. **Use async/await** throughout
2. **Enable query tracking** only when needed
3. **Use projections** in queries
4. **Index frequently queried columns**
5. **Cache static data**
6. **Use batch operations** for bulk updates

---

## 🚨 Troubleshooting

### Build Fails
```bash
# Clean solution
dotnet clean
dotnet restore
dotnet build
```

### Tests Won't Run
```bash
# Ensure test SDK is installed
dotnet --version

# Run with verbose output
dotnet test --verbosity detailed --logger "console;verbosity=detailed"
```

### Database Issues
```bash
# Drop and recreate database
dotnet ef database drop -p Parking.Infrastructure --force
dotnet ef database update -p Parking.Infrastructure
```

### Port Already in Use
```bash
# Use different port
dotnet run -p Parking.API -- --urls="https://localhost:7001"

# Or find and kill process using port 7000
netstat -ano | findstr :7000
taskkill /PID [PID] /F
```

---

## 📖 Documentation Files

| File | Purpose |
|------|---------|
| `README.md` | Comprehensive guide |
| `SETUP_COMPLETE.md` | Setup details |
| `FINAL_REPORT.md` | Complete delivery report |
| `QUICK_START.md` | This file |

---

## 🎯 Development Workflow

1. **Plan**: Define feature in CQRS commands/queries
2. **Domain**: Create entities and value objects
3. **Handlers**: Implement command/query handlers
4. **Repository**: Create repository implementations
5. **Controller**: Add API endpoint
6. **Tests**: Write unit and integration tests
7. **Specs**: Add BDD scenarios
8. **Validation**: Run architecture tests

---

## ✅ Checklist for New Feature

- [ ] Domain entity created
- [ ] Command/Query defined
- [ ] Handler implemented
- [ ] Repository method added
- [ ] Controller endpoint added
- [ ] Unit tests written
- [ ] BDD specs added
- [ ] Architecture tests pass
- [ ] Database migrated
- [ ] Documentation updated

---

## 🔗 Useful Links

- [Microsoft .NET Docs](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [SpecFlow Documentation](https://specflow.org/)
- [Serilog Documentation](https://serilog.net/)

---

## 📞 Quick Help

### Solution won't open?
- Ensure Visual Studio 2022 is installed
- Check .NET 9.0 SDK is installed: `dotnet --version`

### Tests failing?
- Clean and rebuild: `dotnet clean && dotnet build`
- Check database connection strings

### API won't start?
- Ensure port 7000 is available
- Check appsettings.json for correct configuration

---

**Happy coding! 🚀**

For more details, see README.md in the src directory.
