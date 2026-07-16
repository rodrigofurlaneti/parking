namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Purchase.CreatePurchase;
using Parking.Application.Features.Purchase.ReceivePurchaseItems;

[Route("api/[controller]")]
public sealed class PurchaseController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePurchaseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{id}/receive")]
    public async Task<IActionResult> Receive(
        long id,
        [FromBody] ReceivePurchaseItemsBody body,
        CancellationToken cancellationToken)
    {
        var command = new ReceivePurchaseItemsCommand(id, body.Items);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }
}

public sealed record ReceivePurchaseItemsBody(List<ReceivePurchaseItemInput> Items);
