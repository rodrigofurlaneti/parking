namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.ServiceCategory.Create;
using Parking.Application.Features.ServiceCategory.GetAllByBranch;
using Parking.Application.Features.ServiceItem.Create;
using Parking.Application.Features.ServiceItem.GetAll;

[Route("api/services")]
public sealed class ServiceController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(
        CreateServiceCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
        [FromQuery] long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllServiceCategoriesByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem(
        CreateServiceItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems(
        [FromQuery] long categoryId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetServiceItemsQuery(categoryId), cancellationToken);
        return HandleFailure(result);
    }
}
