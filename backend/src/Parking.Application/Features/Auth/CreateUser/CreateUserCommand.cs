namespace Parking.Application.Features.Auth.CreateUser;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateUserCommand(
    string UserName,
    string Email,
    string FullName,
    string Password) : ICommand<UserDto>;
