# Parking - Sistema de Gestao de Estacionamento

Backend em .NET 9 (Clean Architecture) em `backend/src/` e frontend em React + Vite + TypeScript em `frontend/`.

> Este README esta em fase inicial (documentacao completa do projeto e de outra etapa). Por enquanto, contem apenas as instrucoes para rodar o sistema via Docker.

## Rodando com Docker

O projeto possui um `docker-compose.yml` na raiz que sobe 3 containers: banco de dados (SQL Server), API (.NET 9) e frontend (React servido via Nginx).

### Pre-requisitos

- [Docker](https://www.docker.com/) e Docker Compose (`docker compose`) instalados.

### Subindo o ambiente

Na raiz do repositorio, execute:

```bash
docker compose up --build
```

Isso vai:

1. Buildar a imagem da API (`backend/src/Dockerfile`, multi-stage com SDK + ASP.NET runtime).
2. Buildar a imagem do frontend (`frontend/Dockerfile`, multi-stage com Node + Nginx).
3. Subir o SQL Server, aguardar ele ficar saudavel (healthcheck) e so entao subir a API.
4. A API aplica as migrations do Entity Framework Core automaticamente no startup (`APPLY_MIGRATIONS_ON_STARTUP=true`).

### URLs de acesso

| Servico  | URL                                       |
|----------|--------------------------------------------|
| Frontend | http://localhost:3000                      |
| API      | http://localhost:5000                      |
| Swagger  | http://localhost:5000/swagger (se `ASPNETCORE_ENVIRONMENT=Development`) |
| SQL Server | localhost:1433 (usuario `sa`, senha definida em `docker-compose.yml`) |

### Parando o ambiente

```bash
docker compose down
```

Para remover tambem o volume de dados do banco (apaga os dados persistidos):

```bash
docker compose down -v
```

### Variaveis de ambiente relevantes

- `ConnectionStrings__DefaultConnection` (API): string de conexao com o SQL Server do container.
- `APPLY_MIGRATIONS_ON_STARTUP` (API): quando `true`, aplica `dotnet ef database update` (via `Database.Migrate()`) automaticamente ao iniciar. Nao interfere no fluxo de desenvolvimento local (fora do Docker), onde as migrations continuam sendo aplicadas manualmente.
- `VITE_API_URL` (frontend, build-time): URL da API usada pelo bundle do Vite. Definida via `args` no `docker-compose.yml`, pois o Vite injeta variaveis de ambiente no bundle JS em tempo de build, nao de runtime.

As senhas/segredos no `docker-compose.yml` sao valores de exemplo para ambiente local. Nao usar em producao sem troca-los.
