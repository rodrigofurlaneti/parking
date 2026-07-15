namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Company.Create;
using Parking.Application.Features.Company.GetById;

[Route("api/[controller]")]
public sealed class CompanyController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        return HandleFailure(result);
    }
}
