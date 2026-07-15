# Parking System .NET 9 Backend Setup Script
# This script creates a complete .NET 9 solution with all required projects and structure

$backendPath = "C:\Users\AMD\Documents\Claude\Projects\Parking\backend"
$srcPath = Join-Path $backendPath "src"

# Create src directory
Write-Host "Creating src directory..." -ForegroundColor Green
if (-not (Test-Path $srcPath)) {
    New-Item -ItemType Directory -Path $srcPath | Out-Null
}
Set-Location $srcPath

# Step 1: Create solution
Write-Host "Creating Parking.sln solution..." -ForegroundColor Green
dotnet new sln -n Parking

# Step 2: Create all projects
Write-Host "Creating Domain project..." -ForegroundColor Green
dotnet new classlib -n Parking.Domain

Write-Host "Creating Application project..." -ForegroundColor Green
dotnet new classlib -n Parking.Application

Write-Host "Creating Infrastructure project..." -ForegroundColor Green
dotnet new classlib -n Parking.Infrastructure

Write-Host "Creating API project..." -ForegroundColor Green
dotnet new webapi -n Parking.API

Write-Host "Creating Tests project..." -ForegroundColor Green
dotnet new xunit -n Parking.Tests

Write-Host "Creating Specs project..." -ForegroundColor Green
dotnet new classlib -n Parking.Specs

Write-Host "Creating Architecture Tests project..." -ForegroundColor Green
dotnet new classlib -n Parking.ArchTests

# Step 3: Add all projects to solution
Write-Host "Adding projects to solution..." -ForegroundColor Green
dotnet sln add Parking.Domain/Parking.Domain.csproj
dotnet sln add Parking.Application/Parking.Application.csproj
dotnet sln add Parking.Infrastructure/Parking.Infrastructure.csproj
dotnet sln add Parking.API/Parking.API.csproj
dotnet sln add Parking.Tests/Parking.Tests.csproj
dotnet sln add Parking.Specs/Parking.Specs.csproj
dotnet sln add Parking.ArchTests/Parking.ArchTests.csproj

# Step 4: Create project references
Write-Host "Adding project references..." -ForegroundColor Green

# Application -> Domain
dotnet add Parking.Application/Parking.Application.csproj reference Parking.Domain/Parking.Domain.csproj

# Infrastructure -> Domain
dotnet add Parking.Infrastructure/Parking.Infrastructure.csproj reference Parking.Domain/Parking.Domain.csproj

# API -> Application + Infrastructure
dotnet add Parking.API/Parking.API.csproj reference Parking.Application/Parking.Application.csproj
dotnet add Parking.API/Parking.API.csproj reference Parking.Infrastructure/Parking.Infrastructure.csproj

# Tests -> Application + Infrastructure
dotnet add Parking.Tests/Parking.Tests.csproj reference Parking.Application/Parking.Application.csproj
dotnet add Parking.Tests/Parking.Tests.csproj reference Parking.Infrastructure/Parking.Infrastructure.csproj

# Specs -> Application
dotnet add Parking.Specs/Parking.Specs.csproj reference Parking.Application/Parking.Application.csproj

# ArchTests -> All projects
dotnet add Parking.ArchTests/Parking.ArchTests.csproj reference Parking.Domain/Parking.Domain.csproj
dotnet add Parking.ArchTests/Parking.ArchTests.csproj reference Parking.Application/Parking.Application.csproj
dotnet add Parking.ArchTests/Parking.ArchTests.csproj reference Parking.Infrastructure/Parking.Infrastructure.csproj
dotnet add Parking.ArchTests/Parking.ArchTests.csproj reference Parking.API/Parking.API.csproj

# Step 5: Install NuGet packages
Write-Host "Installing NuGet packages for Domain..." -ForegroundColor Green
dotnet add Parking.Domain/Parking.Domain.csproj package MediatR --version 12.3.0
dotnet add Parking.Domain/Parking.Domain.csproj package FluentValidation --version 11.9.2

Write-Host "Installing NuGet packages for Application..." -ForegroundColor Green
dotnet add Parking.Application/Parking.Application.csproj package MediatR --version 12.3.0
dotnet add Parking.Application/Parking.Application.csproj package MediatR.Extensions.Microsoft.DependencyInjection --version 11.2.0
dotnet add Parking.Application/Parking.Application.csproj package AutoMapper --version 13.0.1
dotnet add Parking.Application/Parking.Application.csproj package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add Parking.Application/Parking.Application.csproj package FluentValidation --version 11.9.2
dotnet add Parking.Application/Parking.Application.csproj package FluentValidation.DependencyInjectionExtensions --version 11.9.2

Write-Host "Installing NuGet packages for Infrastructure..." -ForegroundColor Green
dotnet add Parking.Infrastructure/Parking.Infrastructure.csproj package EntityFrameworkCore --version 9.0.0
dotnet add Parking.Infrastructure/Parking.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add Parking.Infrastructure/Parking.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add Parking.Infrastructure/Parking.Infrastructure.csproj package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add Parking.Infrastructure/Parking.Infrastructure.csproj package Microsoft.Extensions.DependencyInjection --version 8.0.0

Write-Host "Installing NuGet packages for API..." -ForegroundColor Green
dotnet add Parking.API/Parking.API.csproj package MediatR --version 12.3.0
dotnet add Parking.API/Parking.API.csproj package MediatR.Extensions.Microsoft.DependencyInjection --version 11.2.0
dotnet add Parking.API/Parking.API.csproj package Serilog --version 3.1.1
dotnet add Parking.API/Parking.API.csproj package Serilog.AspNetCore --version 8.0.1
dotnet add Parking.API/Parking.API.csproj package Serilog.Sinks.Console --version 5.0.1
dotnet add Parking.API/Parking.API.csproj package Serilog.Sinks.File --version 5.0.0

Write-Host "Installing NuGet packages for Tests..." -ForegroundColor Green
dotnet add Parking.Tests/Parking.Tests.csproj package xunit --version 2.7.1
dotnet add Parking.Tests/Parking.Tests.csproj package xunit.runner.visualstudio --version 2.5.6
dotnet add Parking.Tests/Parking.Tests.csproj package Microsoft.NET.Test.Sdk --version 17.9.2
dotnet add Parking.Tests/Parking.Tests.csproj package Moq --version 4.20.70
dotnet add Parking.Tests/Parking.Tests.csproj package FluentAssertions --version 6.12.0

Write-Host "Installing NuGet packages for Specs..." -ForegroundColor Green
dotnet add Parking.Specs/Parking.Specs.csproj package SpecFlow --version 4.0.33
dotnet add Parking.Specs/Parking.Specs.csproj package SpecFlow.xUnit --version 4.0.33
dotnet add Parking.Specs/Parking.Specs.csproj package SpecFlow.Plus.LivingDocPlugin --version 4.0.33

Write-Host "Installing NuGet packages for ArchTests..." -ForegroundColor Green
dotnet add Parking.ArchTests/Parking.ArchTests.csproj package ArchUnitNET --version 2.12.0
dotnet add Parking.ArchTests/Parking.ArchTests.csproj package ArchUnitNET.xUnit --version 2.12.0
dotnet add Parking.ArchTests/Parking.ArchTests.csproj package xunit --version 2.7.1

# Step 6: Create folder structures
Write-Host "Creating folder structures..." -ForegroundColor Green

# Domain structure
$domainDirs = @(
    "Parking.Domain/Entities",
    "Parking.Domain/ValueObjects",
    "Parking.Domain/Interfaces/Repositories",
    "Parking.Domain/Interfaces/Services",
    "Parking.Domain/Exceptions",
    "Parking.Domain/Specifications"
)
foreach ($dir in $domainDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# Application structure
$appDirs = @(
    "Parking.Application/DTOs",
    "Parking.Application/Validators",
    "Parking.Application/Commands/CreateParkingSpace",
    "Parking.Application/Commands/UpdateParkingSpace",
    "Parking.Application/Commands/DeleteParkingSpace",
    "Parking.Application/Commands/CreateReservation",
    "Parking.Application/Commands/CancelReservation",
    "Parking.Application/Queries/GetAvailableSpaces",
    "Parking.Application/Queries/GetReservations",
    "Parking.Application/Handlers/Commands",
    "Parking.Application/Handlers/Queries",
    "Parking.Application/Mappings",
    "Parking.Application/Services",
    "Parking.Application/Specifications"
)
foreach ($dir in $appDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# Infrastructure structure
$infraDirs = @(
    "Parking.Infrastructure/Persistence",
    "Parking.Infrastructure/Persistence/Migrations",
    "Parking.Infrastructure/Persistence/Configurations",
    "Parking.Infrastructure/Repositories",
    "Parking.Infrastructure/Services",
    "Parking.Infrastructure/Interceptors"
)
foreach ($dir in $infraDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# API structure
$apiDirs = @(
    "Parking.API/Controllers",
    "Parking.API/Middleware",
    "Parking.API/Extensions",
    "Parking.API/Filters",
    "Parking.API/Mapping"
)
foreach ($dir in $apiDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# Tests structure
$testsDirs = @(
    "Parking.Tests/Unit",
    "Parking.Tests/Integration",
    "Parking.Tests/Fixtures",
    "Parking.Tests/Mocks"
)
foreach ($dir in $testsDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# Specs structure
$specsDirs = @(
    "Parking.Specs/Features",
    "Parking.Specs/StepDefinitions"
)
foreach ($dir in $specsDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# ArchTests structure
$archDirs = @(
    "Parking.ArchTests/Tests"
)
foreach ($dir in $archDirs) {
    New-Item -ItemType Directory -Path (Join-Path $srcPath $dir) -Force | Out-Null
}

# Step 7: Build solution
Write-Host "Building solution..." -ForegroundColor Green
dotnet build

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Parking Backend Setup Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Solution location: $srcPath" -ForegroundColor Yellow
Write-Host "Total projects created: 7" -ForegroundColor Yellow
Write-Host ""
Write-Host "Projects:" -ForegroundColor Cyan
Write-Host "  - Parking.Domain" -ForegroundColor White
Write-Host "  - Parking.Application" -ForegroundColor White
Write-Host "  - Parking.Infrastructure" -ForegroundColor White
Write-Host "  - Parking.API" -ForegroundColor White
Write-Host "  - Parking.Tests" -ForegroundColor White
Write-Host "  - Parking.Specs" -ForegroundColor White
Write-Host "  - Parking.ArchTests" -ForegroundColor White
Write-Host "============================================" -ForegroundColor Cyan

Read-Host "Press Enter to exit"
