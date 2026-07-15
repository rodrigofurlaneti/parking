namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Branch.Create;
using Parking.Application.Features.Branch.GetByCompany;

[Route("api/[controller]")]
public sealed class BranchController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBranchCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCompany(
        [FromQuery] long companyId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBranchesByCompanyQuery(companyId), cancellationToken);
        return HandleFailure(result);
    }
}
