namespace Parking.Application.Features.Auth.CreateUser;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;
using Parking.Domain.ValueObjects;

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    private readonly IAppUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IAppUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicates
        if (await _userRepository.ExistsAsync(request.UserName, request.Email, cancellationToken))
            return Result.Failure<UserDto>(new Error("AppUser.DuplicateEmail", "User already exists."));

        // Create value objects
        var usernameResult = Username.Create(request.UserName);
        if (usernameResult.IsFailure)
            return Result.Failure<UserDto>(usernameResult.Error);

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<UserDto>(emailResult.Error);

        // Create user
        var userResult = AppUser.Create(
            usernameResult.Value,
            emailResult.Value,
            _passwordHasher.Hash(request.Password),
            request.FullName);

        if (userResult.IsFailure)
            return Result.Failure<UserDto>(userResult.Error);

        await _userRepository.AddAsync(userResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new UserDto(
            userResult.Value.Id,
            userResult.Value.UserName.Value,
            userResult.Value.Email.Value,
            userResult.Value.FullName,
            userResult.Value.IsActive,
            new List<string>()));
    }
}
