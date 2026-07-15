namespace Parking.Application.Features.Auth.AssignRole;

using Parking.Application.Abstractions.Messaging;

public sealed record AssignRoleCommand(
    long UserId,
    long RoleId) : ICommand;
