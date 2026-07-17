namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Supplier.CreateSupplier;
using Parking.Application.Features.Supplier.DeactivateSupplier;
using Parking.Application.Features.Supplier.GetAllByBranch;
using Parking.Application.Features.Supplier.UpdateSupplier;

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

    [HttpPut("{supplierId}")]
    public async Task<IActionResult> Update(
        long supplierId,
        UpdateSupplierCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { SupplierId = supplierId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{supplierId}/deactivate")]
    public async Task<IActionResult> Deactivate(
        long supplierId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateSupplierCommand(supplierId), cancellationToken);
        return HandleFailure(result);
    }
}
