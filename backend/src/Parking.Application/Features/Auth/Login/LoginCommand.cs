namespace Parking.Application.Features.Auth.Login;

using Parking.Application.Abstractions.Messaging;

public sealed record LoginCommand(
    string UserName,
    string Password) : ICommand<LoginResponse>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    long UserId,
    string UserName);
