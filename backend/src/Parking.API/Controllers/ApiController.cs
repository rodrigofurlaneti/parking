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
            _ => ToErrorResult(result.Error)
        };

    protected IActionResult HandleFailure<T>(Result<T> result) =>
        result switch
        {
            { IsSuccess: true, } success => Ok(((dynamic)success).Value),
            _ => ToErrorResult(result.Error)
        };

    // Mapeia o Error do domínio/aplicação para uma resposta HTTP com o codigo e a
    // mensagem reais, em vez do "Operation failed" generico que escondia a causa de
    // toda falha na API (ex.: tarifa nao configurada, cliente nao encontrado, etc.).
    private IActionResult ToErrorResult(Error error)
    {
        var body = new { error = error.Message, code = error.Code };

        if (error.Code.EndsWith(".NotFound", StringComparison.Ordinal))
            return NotFound(body);

        if (error.Code.EndsWith(".AlreadyExited", StringComparison.Ordinal)
            || error.Code.EndsWith(".AlreadyParked", StringComparison.Ordinal)
            || error.Code.EndsWith(".AlreadyClosed", StringComparison.Ordinal)
            || error.Code.Contains("Duplicate", StringComparison.Ordinal))
            return Conflict(body);

        return BadRequest(body);
    }
}
