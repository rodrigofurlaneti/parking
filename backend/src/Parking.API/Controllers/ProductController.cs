namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Product.Create;
using Parking.Application.Features.Product.GetStock;

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
}
