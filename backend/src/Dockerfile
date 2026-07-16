# syntax=docker/dockerfile:1

# ==========================================================================
# Stage 1: build - compila e publica a Parking.API (e dependencias)
# ==========================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os .csproj primeiro para aproveitar o cache de layer do
# "dotnet restore" (so reexecuta o restore se as dependencias mudarem).
COPY Parking.sln ./
COPY Directory.Build.props ./
COPY Parking.Domain/Parking.Domain.csproj Parking.Domain/
COPY Parking.Application/Parking.Application.csproj Parking.Application/
COPY Parking.Infrastructure/Parking.Infrastructure.csproj Parking.Infrastructure/
COPY Parking.API/Parking.API.csproj Parking.API/
COPY Parking.Tests/Parking.Tests.csproj Parking.Tests/
COPY Parking.Specs/Parking.Specs.csproj Parking.Specs/
COPY Parking.ArchTests/Parking.ArchTests.csproj Parking.ArchTests/

RUN dotnet restore Parking.API/Parking.API.csproj

# Copia o restante do codigo-fonte e publica somente a API (Release).
COPY . .
RUN dotnet publish Parking.API/Parking.API.csproj -c Release -o /app/publish --no-restore

# ==========================================================================
# Stage 2: final - imagem de runtime enxuta (ASP.NET, sem SDK)
# ==========================================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Parking.API.dll"]
