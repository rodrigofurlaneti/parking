namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Sale.GetSaleById;
using Parking.Application.Features.Sale.RegisterSale;

[Route("api/[controller]")]
public sealed class SaleController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Register(
        RegisterSaleCommand command,
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
        var result = await Mediator.Send(new GetSaleByIdQuery(id), cancellationToken);
        return HandleFailure(result);
    }
}
