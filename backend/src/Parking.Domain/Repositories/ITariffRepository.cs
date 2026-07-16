namespace Parking.Domain.Repositories;

using Parking.Domain.Entities;

public interface ITariffRepository
{
    // Simplificacao: retorna a unica tarifa ATIVA da filial (nao ha historico de tarifas por data).
    Task<Tariff?> GetActiveByBranchAsync(long branchId, CancellationToken ct = default);
    Task<Tariff?> GetByIdAsync(long id, CancellationToken ct = default);
    Task AddAsync(Tariff entity, CancellationToken ct = default);
    Task UpdateAsync(Tariff entity, CancellationToken ct = default);
}
