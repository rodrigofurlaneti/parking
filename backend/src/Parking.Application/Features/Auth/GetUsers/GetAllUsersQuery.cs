namespace Parking.Application.Features.Auth.GetUsers;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllUsersQuery : IQuery<List<UserDto>>;
