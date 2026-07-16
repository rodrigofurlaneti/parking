namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Product.AdjustStock;
using Parking.Application.Features.Product.Create;
using Parking.Application.Features.Product.GetBelowMinimum;
using Parking.Application.Features.Product.GetStock;
using Parking.Application.Features.Product.GetStockLedger;

[Route("api/products")]
public sealed class ProductController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("stock")]
    public async Task<IActionResult> GetStock(
        [FromQuery] long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetProductStockQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{id}/adjust-stock")]
    public async Task<IActionResult> AdjustStock(
        long id,
        [FromBody] AdjustStockBody body,
        CancellationToken cancellationToken)
    {
        var command = new AdjustStockCommand(id, body.Adjustment, body.Reason);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{id}/stock-ledger")]
    public async Task<IActionResult> GetStockLedger(
        long id,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetStockLedgerQuery(id, fromDate, toDate), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("below-minimum")]
    public async Task<IActionResult> GetBelowMinimum(
        [FromQuery] long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBelowMinimumQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }
}

public sealed record AdjustStockBody(decimal Adjustment, string Reason);
