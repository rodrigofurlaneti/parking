namespace Parking.Application.Features.Auth.RefreshToken;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Auth.Login;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<LoginResponse>;
