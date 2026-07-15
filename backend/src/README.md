# Parking System - Backend Solution

Complete .NET 9 backend solution for the Parking management system with clean architecture, CQRS pattern, and comprehensive testing.

## Projects Overview

### 1. **Parking.Domain**
- Core business logic and domain entities
- Value objects and domain specifications
- Interfaces for repositories and services
- Dependencies: MediatR, FluentValidation

### 2. **Parking.Application**
- CQRS commands and queries
- Application services and handlers
- DTOs and validators
- AutoMapper configurations
- Dependencies: Parking.Domain, MediatR, AutoMapper, FluentValidation

### 3. **Parking.Infrastructure**
- Entity Framework Core data access layer
- Repository pattern implementations
- Database configurations and migrations
- Third-party service integrations
- Dependencies: Parking.Domain, EF Core, SQL Server

### 4. **Parking.API**
- ASP.NET Core REST API controllers
- Middleware and filters
- OpenAPI/Swagger documentation
- Serilog logging configuration
- Dependencies: Parking.Application, Parking.Infrastructure, Serilog

### 5. **Parking.Tests**
- Unit tests with xUnit
- Integration tests
- Mocking with Moq
- Assertions with FluentAssertions
- Dependencies: Parking.Application, Parking.Infrastructure

### 6. **Parking.Specs**
- BDD specifications with SpecFlow
- Feature files
- Step definitions
- Dependencies: Parking.Application

### 7. **Parking.ArchTests**
- Architecture tests with ArchUnitNET
- Layering and dependency rules
- Naming convention validation
- Dependencies: All projects

## Project Structure

```
src/
├── Parking.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Services/
│   ├── Exceptions/
│   └── Specifications/
├── Parking.Application/
│   ├── DTOs/
│   ├── Validators/
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── Mappings/
│   ├── Services/
│   └── Specifications/
├── Parking.Infrastructure/
│   ├── Persistence/
│   │   ├── Migrations/
│   │   └── Configurations/
│   ├── Repositories/
│   ├── Services/
│   └── Interceptors/
├── Parking.API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Extensions/
│   ├── Filters/
│   ├── Mapping/
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Parking.Tests/
│   ├── Unit/
│   ├── Integration/
│   ├── Fixtures/
│   └── Mocks/
├── Parking.Specs/
│   ├── Features/
│   └── StepDefinitions/
└── Parking.ArchTests/
    └── Tests/
```

## Technology Stack

- **.NET 9.0**
- **Entity Framework Core 9.0** - ORM
- **MediatR 12.3.0** - CQRS implementation
- **AutoMapper 13.0.1** - Object mapping
- **FluentValidation 11.9.2** - Input validation
- **Serilog 3.1.1** - Structured logging
- **xUnit 2.7.1** - Unit testing framework
- **Moq 4.20.70** - Mocking library
- **SpecFlow 4.0.33** - BDD testing
- **ArchUnitNET 2.12.0** - Architecture testing
- **SQL Server** - Database

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full installation)
- Visual Studio 2022 or VS Code

### Build and Run

```bash
# Build the solution
dotnet build

# Run tests
dotnet test

# Start the API
cd Parking.API
dotnet run
```

### API Endpoints

Once running, the API will be available at:
- **Base URL**: https://localhost:7000
- **Swagger UI**: https://localhost:7000/swagger

## Development Guidelines

### Architecture Principles
1. **Clean Architecture**: Separation of concerns across layers
2. **CQRS Pattern**: Separate read and write operations
3. **Repository Pattern**: Data access abstraction
4. **Dependency Injection**: Loose coupling through interfaces

### Code Standards
- Follow C# naming conventions
- Use async/await for I/O operations
- Implement proper exception handling
- Write comprehensive tests
- Document public APIs

## Database

Connection string (appsettings.json):
```
Server=(localdb)\mssqllocaldb;Database=ParkingDb;Trusted_Connection=true;
```

### EF Core Commands
```bash
# Add migration
dotnet ef migrations add MigrationName -p Parking.Infrastructure

# Update database
dotnet ef database update -p Parking.Infrastructure
```

## Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Project
```bash
dotnet test Parking.Tests/
dotnet test Parking.Specs/
dotnet test Parking.ArchTests/
```

## Logging

Logs are configured via Serilog and saved to:
- **Development**: Console and file (`logs/parking-api-*.txt`)
- **Production**: File with daily rolling intervals

## Next Steps

1. Implement domain entities and value objects
2. Create EF Core DbContext and configurations
3. Implement repositories
4. Add CQRS commands and queries
5. Develop API controllers
6. Write comprehensive tests
7. Add BDD specifications
8. Validate architecture rules

## Author
Parking System Development Team

## License
Proprietary
