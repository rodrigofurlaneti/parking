namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Domain.Common;

// [Authorize] aqui cobre TODOS os controllers da API que herdam de ApiController (todos exceto
// HealthController, que herda ControllerBase diretamente e permanece publico de proposito).
// AuthController (que tambem herda ApiController) marca explicitamente com [AllowAnonymous]
// as acoes que um usuario nao autenticado precisa chamar (register/login/refresh-token).
[ApiController]
[Authorize]
public abstract class ApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator => mediator;

    protected IActionResult HandleFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => Ok(),
            _ => BadRequest(new { error = "Operation failed" })
        };

    protected IActionResult HandleFailure<T>(Result<T> result) =>
        result switch
        {
            { IsSuccess: true, } success => Ok(((dynamic)success).Value),
            _ => BadRequest(new { error = "Operation failed" })
        };
}
