using Microsoft.AspNetCore.Mvc;

namespace Parking.API.Controllers;

/// <summary>
/// Health check controller
/// </summary>
[ApiController]
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
