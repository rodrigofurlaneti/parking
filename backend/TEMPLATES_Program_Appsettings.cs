/*
 * TEMPLATES_Program_Appsettings.cs
 *
 * Program.cs and appsettings.json templates for Fase 1 API setup.
 *
 * Instructions:
 * 1. Copy the Program.cs code to Parking.API/Program.cs
 * 2. Create Parking.API/appsettings.json with the JSON below
 * 3. Create Parking.API/appsettings.Development.json for local dev
 * 4. Update ConnectionString with your SQL Server details
 * 5. Update Jwt secret (use a strong 32+ character random string)
 */

// ═══════════════════════════════════════════════════════════════════════════
// FILE: Parking.API/Program.cs
// ═══════════════════════════════════════════════════════════════════════════

using Parking.API.Middleware;
using Parking.Application;
using Parking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS (if needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new System.IdentityModel.Tokens.Jwt.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(secretKey ?? "your-secret-key-must-be-at-least-32-characters-long"))
        };
    });

builder.Services.AddAuthorization();

// Add Serilog logging
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

// ═══════════════════════════════════════════════════════════════════════════
// FILE: Parking.API/appsettings.json
// ═══════════════════════════════════════════════════════════════════════════

/*
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ParkingDb;Trusted_Connection=true;Encrypt=false;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "your-secret-key-must-be-at-least-32-characters-long-and-secure",
    "Issuer": "ParkingApi",
    "Audience": "ParkingClient",
    "ExpiryMinutes": 15
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
*/

// ═══════════════════════════════════════════════════════════════════════════
// FILE: Parking.API/appsettings.Development.json
// ═══════════════════════════════════════════════════════════════════════════

/*
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ParkingDb_Dev;Trusted_Connection=true;Encrypt=false;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "dev-secret-key-32-characters-minimum-for-development-only",
    "Issuer": "ParkingApi",
    "Audience": "ParkingClient",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
*/

// ═══════════════════════════════════════════════════════════════════════════
// INFRASTRUCTURE DEPENDENCY INJECTION EXTENSION
// ═══════════════════════════════════════════════════════════════════════════

/*
File: Parking.Infrastructure/DependencyInjection.cs

namespace Parking.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parking.Application.Abstractions.Services;
using Parking.Domain.Repositories;
using Parking.Infrastructure.Persistence;
using Parking.Infrastructure.Persistence.Repositories;
using Parking.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // HttpContextAccessor for CurrentUserService
        services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}
*/

// ═══════════════════════════════════════════════════════════════════════════
// REQNROLL BDD FEATURES
// ═══════════════════════════════════════════════════════════════════════════

/*
File: Parking.Specs/Features/Authentication.feature

Feature: User Authentication
  As a parking system user
  I want to authenticate and manage sessions
  So that I can access the system securely

  Scenario: Register a new user
    Given no user exists with email "john@example.com"
    When I register with username "john" and password "SecurePass123"
    Then the user should be created successfully
    And I should be able to login with those credentials

  Scenario: Login with valid credentials
    Given a user exists with username "john" and password "SecurePass123"
    When I login with username "john" and password "SecurePass123"
    Then I should receive a valid JWT access token
    And the token should contain my username claim

  Scenario: Login with invalid password
    Given a user exists with username "john" and password "SecurePass123"
    When I login with username "john" and password "WrongPassword"
    Then I should receive an error "Invalid username or password"
    And I should not receive an access token

  Scenario: Assign role to user
    Given a user exists with username "admin"
    And a role exists with name "Administrator"
    When I assign the role "Administrator" to user "admin"
    Then the user should have the role "Administrator"
    And the user should have all permissions of that role
*/

/*
File: Parking.Specs/Features/Company.feature

Feature: Company Management
  As a parking operator
  I want to create and manage parking companies
  So that I can organize multiple parking lots

  Scenario: Create a new company
    Given no company exists with CNPJ "12345678901234"
    When I create a company with:
      | Field     | Value                 |
      | Name      | Parking Plus          |
      | CNPJ      | 12345678901234        |
      | LegalName | Parking Plus LTDA     |
    Then the company should be created successfully
    And the company should be active

  Scenario: Create branch for company
    Given a company exists with CNPJ "12345678901234"
    When I create a branch with:
      | Field      | Value          |
      | CompanyId  | 1              |
      | Name       | Centro         |
      | TotalSpaces| 100            |
    Then the branch should be created successfully
    And the branch should be associated with the company
*/

// ═══════════════════════════════════════════════════════════════════════════
// REQNROLL STEP DEFINITIONS (Exemplo)
// ═══════════════════════════════════════════════════════════════════════════

/*
File: Parking.Specs/StepDefinitions/AuthenticationSteps.cs

using System;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class AuthenticationSteps
{
    private string? _registeredUsername;
    private string? _registeredEmail;
    private string? _lastError;
    private string? _accessToken;

    [Given(@"no user exists with email ""(.*)""")]
    public void GivenNoUserExistsWithEmail(string email)
    {
        // TODO: Query database to verify user doesn't exist
        _registeredEmail = null;
    }

    [When(@"I register with username ""(.*)"" and password ""(.*)""")]
    public void WhenIRegisterWithUsernameAndPassword(string username, string password)
    {
        // TODO: Call CreateUserCommand via HTTP/handler
        _registeredUsername = username;
    }

    [Then(@"the user should be created successfully")]
    public void ThenTheUserShouldBeCreatedSuccessfully()
    {
        // TODO: Verify user was created
        Assert.NotNull(_registeredUsername);
    }

    [Given(@"a user exists with username ""(.*)"" and password ""(.*)""")]
    public void GivenAUserExistsWithUsernameAndPassword(string username, string password)
    {
        // TODO: Create user in database or via API
    }

    [When(@"I login with username ""(.*)"" and password ""(.*)""")]
    public void WhenILoginWithUsernameAndPassword(string username, string password)
    {
        // TODO: Call LoginCommand via HTTP/handler
    }

    [Then(@"I should receive a valid JWT access token")]
    public void ThenIShouldReceiveAValidJWTAccessToken()
    {
        // TODO: Verify token is valid and not expired
        Assert.NotNull(_accessToken);
    }

    [Then(@"I should receive an error ""(.*)""")]
    public void ThenIShouldReceiveAnError(string errorMessage)
    {
        Assert.NotNull(_lastError);
        Assert.Contains(errorMessage, _lastError);
    }
}
*/

/*
File: Parking.Specs/StepDefinitions/CompanySteps.cs

using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

[Binding]
public class CompanySteps
{
    private Dictionary<string, string>? _companyData;
    private long _createdCompanyId;
    private string? _lastError;

    [Given(@"no company exists with CNPJ ""(.*)""")]
    public void GivenNoCompanyExistsWithCNPJ(string cnpj)
    {
        // TODO: Query database to verify company doesn't exist
    }

    [When(@"I create a company with:")]
    public void WhenICreateACompanyWith(Table table)
    {
        _companyData = table.Rows[0].Values.ToDictionary(x => x, x => table.Rows[0][x]);
        // TODO: Call CreateCompanyCommand
        _createdCompanyId = 1; // Simulated
    }

    [Then(@"the company should be created successfully")]
    public void ThenTheCompanyShouldBeCreatedSuccessfully()
    {
        Assert.True(_createdCompanyId > 0);
    }

    [Then(@"the company should be active")]
    public void ThenTheCompanyShouldBeActive()
    {
        // TODO: Query database and verify IsActive = true
    }

    [Given(@"a company exists with CNPJ ""(.*)""")]
    public void GivenACompanyExistsWithCNPJ(string cnpj)
    {
        // TODO: Create company in database
    }

    [When(@"I create a branch with:")]
    public void WhenICreateABranchWith(Table table)
    {
        var branchData = table.Rows[0].Values.ToDictionary(x => x, x => table.Rows[0][x]);
        // TODO: Call CreateBranchCommand
    }

    [Then(@"the branch should be created successfully")]
    public void ThenTheBranchShouldBeCreatedSuccessfully()
    {
        // TODO: Verify branch was created
    }

    [Then(@"the branch should be associated with the company")]
    public void ThenTheBranchShouldBeAssociatedWithTheCompany()
    {
        // TODO: Verify FK relationship
    }
}
*/

// ═══════════════════════════════════════════════════════════════════════════
// ARCHITECTURE TESTS (NetArchTest)
// ═══════════════════════════════════════════════════════════════════════════

/*
File: Parking.ArchTests/ArchitectureTests.cs

using NetArchTest.Rules;
using Parking.Application;
using Parking.Domain;
using Parking.Infrastructure;
using Parking.API;
using Xunit;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotHaveDependencies()
    {
        var result = Types.InAssembly(typeof(Entity).Assembly)
            .Should()
            .NotHaveDependencyOn("MediatR")
            .And()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .And()
            .NotHaveDependencyOn("FluentValidation")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join("\n", result.FailingTypes));
    }

    [Fact]
    public void Application_ShouldNotReferencePersistenceOrWeb()
    {
        var result = Types.InAssembly(typeof(ICommand).Assembly)
            .Should()
            .NotHaveDependencyOn("Parking.Infrastructure")
            .And()
            .NotHaveDependencyOn("Parking.API")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join("\n", result.FailingTypes));
    }

    [Fact]
    public void Handlers_ShouldBeInternalSealed()
    {
        var result = Types.InAssembly(typeof(ICommandHandler<>).Assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Repositories_ShouldBeInternalSealed()
    {
        var result = Types.InAssembly(typeof(CompanyRepository).Assembly)
            .That()
            .HaveName(".*Repository")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DTOs_ShouldBeSealed()
    {
        var result = Types.InAssembly(typeof(UserDto).Assembly)
            .That()
            .HaveName("*Dto")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
*/

// ═══════════════════════════════════════════════════════════════════════════
// QUICK REFERENCE: Key Endpoints
// ═══════════════════════════════════════════════════════════════════════════

/*
POST /api/auth/register
Body: { "userName": "john", "email": "john@example.com", "fullName": "John Doe", "password": "Password123" }
Response: { "id": 1, "userName": "john", "email": "john@example.com", ... }

POST /api/auth/login
Body: { "userName": "john", "password": "Password123" }
Response: { "accessToken": "eyJ...", "refreshToken": "...", "userId": 1, "userName": "john" }

POST /api/auth/refresh-token
Body: { "refreshToken": "..." }
Response: { "accessToken": "eyJ...", ... }

POST /api/auth/assign-role
Body: { "userId": 1, "roleId": 1 }
Response: 200 OK

GET /api/auth/users
Headers: Authorization: Bearer <token>
Response: [ { "id": 1, "userName": "john", ... }, ... ]

POST /api/companies
Body: { "name": "Parking Plus", "cnpj": "12345678901234", "legalName": "Parking Plus LTDA" }
Response: { "id": 1, "name": "Parking Plus", ... }

GET /api/companies/1
Response: { "id": 1, "name": "Parking Plus", ... }

POST /api/branches
Body: { "companyId": 1, "name": "Centro", "totalSpaces": 100 }
Response: { "id": 1, "companyId": 1, "name": "Centro", ... }

GET /api/branches?companyId=1
Response: [ { "id": 1, "companyId": 1, "name": "Centro", ... }, ... ]
*/
