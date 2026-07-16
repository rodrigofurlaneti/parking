namespace Parking.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);

    // Abstraction for an ambient database transaction spanning multiple CommitAsync calls.
    // Deliberately does NOT expose EF Core's IDbContextTransaction (Microsoft.EntityFrameworkCore.Storage)
    // here, because Parking.Domain must not depend on EF Core (enforced by ArchitectureTests).
    // The concrete transaction handling lives in the Infrastructure implementation (AppDbContext).
    Task BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(CancellationToken ct = default);

    Task RollbackTransactionAsync(CancellationToken ct = default);
}
