namespace Parking.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class AppUserRepository(AppDbContext context) : IAppUserRepository
{
    public async Task<AppUser?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.AppUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<AppUser?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return await context.AppUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName.Value == userName && x.IsActive, ct);
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await context.AppUsers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.Value == email && x.IsActive, ct);
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.AppUsers.AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(string userName, string email, CancellationToken ct = default)
    {
        return await context.AppUsers
            .AnyAsync(x => (x.UserName.Value == userName || x.Email.Value == email), ct);
    }

    public async Task AddAsync(AppUser entity, CancellationToken ct = default)
    {
        await context.AppUsers.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(AppUser entity, CancellationToken ct = default)
    {
        context.AppUsers.Update(entity);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(long userId, CancellationToken ct = default)
    {
        return await (
            from userRole in context.UserRoles.AsNoTracking()
            join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
            where userRole.UserId == userId && role.IsActive
            select role.Name)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task AddRoleToUserAsync(long userId, long roleId, CancellationToken ct = default)
    {
        var alreadyAssigned = await context.UserRoles
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId, ct);

        if (!alreadyAssigned)
        {
            await context.UserRoles.AddAsync(new UserRole(userId, roleId), ct);
        }
    }
}
