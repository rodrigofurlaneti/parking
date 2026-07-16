namespace Parking.Application.Features.Auth.AssignRole;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand>
{
    private readonly IAppUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(
        IAppUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new Error("AppUser.NotFound", "User not found."));

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "Role not found."));

        // AppUser.AssignRole() e apenas um marcador de auditoria (toca UpdatedAt) - o vinculo
        // real usuario-role e persistido via UserRole (tabela de junção), pois nesse modelo
        // UserRole nao e uma entidade filha da agregacao AppUser.
        user.AssignRole(request.RoleId);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.AddRoleToUserAsync(request.UserId, request.RoleId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
