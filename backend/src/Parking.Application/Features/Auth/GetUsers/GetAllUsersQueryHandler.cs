namespace Parking.Application.Features.Auth.GetUsers;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IAppUserRepository _userRepository;

    public GetAllUsersQueryHandler(IAppUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var dtos = users.Select(u => new UserDto(
            u.Id,
            u.UserName.Value,
            u.Email.Value,
            u.FullName,
            u.IsActive,
            new List<string>())).ToList();

        return Result.Success(dtos);
    }
}
