# CI/CD - Parking

Pipeline no mesmo padrao dos servicos irmaos (EmailSendingService, SmsSendingService,
WhatsAppSendingService). A Parking.API e publicada na **mesma VM**, na **porta 83**.

Mapa de portas na VM:

| Servico                 | Porta host |
|-------------------------|------------|
| EmailSendingService     | 80         |
| SmsSendingService       | 81         |
| WhatsAppSendingService  | 82         |
| **Parking**             | **83**     |

## Fluxo

1. **CI** (`.github/workflows/ci.yml`) — em cada push/PR na `main`: builda e testa o
   backend (`backend/src/Parking.sln`, .NET 9) e o frontend (React + Vite).
2. **CD** (`.github/workflows/deploy.yml`) — dispara automaticamente quando o **CI**
   termina verde na `main` (ou manualmente em Actions):
   - builda a imagem Docker (contexto `backend/src`) e faz push no GHCR
     (`ghcr.io/rodrigofurlaneti/parking:latest`);
   - conecta via SSH na VM, faz `docker pull` e recria o container `parking-api`
     em `-p 83:8080` com `--env-file /opt/parkingservice/.env`;
   - valida o health em `http://localhost:83/api/health`.

## Secrets do repositorio (Settings > Secrets and variables > Actions)

Os mesmos usados pelos servicos irmaos:

- `VM_HOST` — IP/host da VM
- `VM_USER` — usuario SSH
- `VM_SSH_KEY` — chave privada SSH
- `GHCR_PAT` — Personal Access Token com escopo `read:packages` (pull na VM)

## Preparacao na VM (uma vez)

```bash
sudo mkdir -p /opt/parkingservice
sudo cp deploy/.env.example /opt/parkingservice/.env
sudo nano /opt/parkingservice/.env   # preencher conn string do SQL e Jwt__Secret
```

A porta 83 precisa estar liberada no firewall / NSG da VM.

> A Parking.API depende de um SQL Server. O `.env` deve apontar
> `ConnectionStrings__DefaultConnection` para uma instancia acessivel pela VM.
> Com `APPLY_MIGRATIONS_ON_STARTUP=true`, as migrations do EF Core sao aplicadas
> automaticamente no startup do container.
