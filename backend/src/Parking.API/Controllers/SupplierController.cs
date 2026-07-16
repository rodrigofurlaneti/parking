namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Supplier.CreateSupplier;
using Parking.Application.Features.Supplier.GetAllByBranch;

[Route("api/[controller]")]
public sealed class SupplierController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateSupplierCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByBranch(
        [FromQuery] long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllSuppliersByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }
}
