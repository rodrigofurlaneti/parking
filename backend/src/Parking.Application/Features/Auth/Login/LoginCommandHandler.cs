namespace Parking.Application.Features.Auth.Login;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Abstractions.Services;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAppUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IAppUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(
                new Error("Auth.InvalidCredentials", "Invalid username or password."));

        if (!user.IsActive)
            return Result.Failure<LoginResponse>(
                new Error("Auth.UserInactive", "User account is inactive."));

        if (user.IsLockedOut)
            return Result.Failure<LoginResponse>(
                new Error("Auth.UserLockedOut", "User account is locked out."));

        // Reset failed attempts
        user.ResetFailedAccessCount();
        user.SetLastLogin();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.UserName.Value, user.Email.Value);

        // Create refresh token (simplified - should use RefreshToken entity)
        var refreshToken = Guid.NewGuid().ToString("N");

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new LoginResponse(accessToken, refreshToken, user.Id, user.UserName.Value));
    }
}
