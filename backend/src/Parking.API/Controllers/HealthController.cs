using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parking.API.Controllers;

/// <summary>
/// Health check controller. Deve permanecer publico - usado por healthchecks de infraestrutura
/// (Docker/orquestrador). Herda ControllerBase diretamente (nao ApiController), entao nunca
/// receberia [Authorize] por heranca, mas o [AllowAnonymous] fica explicito aqui por clareza.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Get API health status
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
    }
}
