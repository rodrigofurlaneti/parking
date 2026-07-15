namespace Parking.Application.Features.Auth.RefreshToken;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Abstractions.Services;
using Parking.Application.Features.Auth.Login;
using Parking.Domain.Common;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // This is a simplified version - in real app, validate refresh token against DB
        return Task.FromResult(Result.Failure<LoginResponse>(
            new Error("Auth.InvalidRefreshToken", "Invalid or expired refresh token.")));
    }
}
