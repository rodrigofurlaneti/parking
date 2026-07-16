namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Customer.CreateCustomer;
using Parking.Application.Features.Customer.GetAllByBranch;
using Parking.Application.Features.Customer.GetCustomerByDocument;

[Route("api/[controller]")]
public sealed class CustomerController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCustomerCommand command,
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
        var result = await Mediator.Send(new GetAllCustomersByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("document/{document}")]
    public async Task<IActionResult> GetByDocument(
        string document,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCustomerByDocumentQuery(document), cancellationToken);
        return HandleFailure(result);
    }
}
