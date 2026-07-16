namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Auth.AssignRole;
using Parking.Application.Features.Auth.CreateUser;
using Parking.Application.Features.Auth.GetUsers;
using Parking.Application.Features.Auth.Login;
using Parking.Application.Features.Auth.RefreshToken;

// A classe herda [Authorize] de ApiController. As tres acoes abaixo (register/login/refresh-token)
// sao as unicas que um usuario nao autenticado precisa poder chamar, entao recebem [AllowAnonymous]
// explicito. assign-role e users (GetUsers) permanecem exigindo autenticacao (fallback [Authorize]
// herdado da base): nao ha, hoje, nenhuma role "Admin" seedada/garantida na tabela Role, entao usar
// [Authorize(Roles = "Admin")] aqui poderia bloquear todo mundo por engano. Quando existir um nome de
// role administrativo confirmado (seed/migration), trocar para [Authorize(Roles = "Admin")].
[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ApiController(mediator)
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(
        AssignRoleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return HandleFailure(result);
    }
}
