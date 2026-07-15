namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Domain.Common;

[ApiController]
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
