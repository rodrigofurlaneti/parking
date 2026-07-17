namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Branch.Create;
using Parking.Application.Features.Branch.Deactivate;
using Parking.Application.Features.Branch.GetByCompany;
using Parking.Application.Features.Branch.Update;

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

    [HttpPut("{branchId}")]
    public async Task<IActionResult> Update(
        long branchId,
        UpdateBranchCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { BranchId = branchId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{branchId}/deactivate")]
    public async Task<IActionResult> Deactivate(
        long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateBranchCommand(branchId), cancellationToken);
        return HandleFailure(result);
    }
}
