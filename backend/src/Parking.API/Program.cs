using Microsoft.EntityFrameworkCore;
using Parking.Application;
using Parking.Infrastructure;
using Parking.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/parking-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Parking System API",
        Version = "v1"
    });

    // Permite testar endpoints protegidos direto pela UI do Swagger, informando "Bearer {token}".
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Informe o token JWT obtido em POST /api/auth/login. Ex: Bearer eyJhbGciOi...",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.SetIsOriginAllowed(origin =>
                Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                (uri.Host == "localhost" || uri.Host == "127.0.0.1"))
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Aplica as migrations do EF Core automaticamente no startup.
// Util em containers Docker, onde nao ha um passo manual de "dotnet ef database update".
// Nao interfere no fluxo de desenvolvimento local (que continua usando o comando manual),
// pois so roda em Production ou quando a env var APPLY_MIGRATIONS_ON_STARTUP=true for definida
// explicitamente (ex.: no docker-compose.yml).
if (app.Environment.IsProduction() ||
    Environment.GetEnvironmentVariable("APPLY_MIGRATIONS_ON_STARTUP") == "true")
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Log.Information("Applying pending EF Core migrations...");
    dbContext.Database.Migrate();
    Log.Information("Migrations applied successfully.");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Parking System API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Parking System API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
