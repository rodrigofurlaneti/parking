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

# Restaura somente a API (traz as dependencias transitivas). Os projetos de
# teste ficam em backend/test (fora deste contexto) e nao entram na imagem.
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

# A API escuta na 8080 dentro do container e roda em modo Production
# (dispara a aplicacao automatica das migrations do EF Core no startup).
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Healthcheck usando o endpoint publico HealthController (rota api/health).
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
  CMD wget -qO- http://localhost:8080/api/health || exit 1

ENTRYPOINT ["dotnet", "Parking.API.dll"]
