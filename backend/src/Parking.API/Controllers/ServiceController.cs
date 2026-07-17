namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.ServiceCategory.Create;
using Parking.Application.Features.ServiceCategory.Deactivate;
using Parking.Application.Features.ServiceCategory.GetAllByBranch;
using Parking.Application.Features.ServiceCategory.Update;
using Parking.Application.Features.ServiceItem.Create;
using Parking.Application.Features.ServiceItem.Deactivate;
using Parking.Application.Features.ServiceItem.GetAll;
using Parking.Application.Features.ServiceItem.Update;

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

    [HttpPut("categories/{categoryId}")]
    public async Task<IActionResult> UpdateCategory(
        long categoryId,
        UpdateServiceCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { ServiceCategoryId = categoryId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("categories/{categoryId}/deactivate")]
    public async Task<IActionResult> DeactivateCategory(
        long categoryId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateServiceCategoryCommand(categoryId), cancellationToken);
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

    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateItem(
        long itemId,
        UpdateServiceItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { ServiceItemId = itemId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("items/{itemId}/deactivate")]
    public async Task<IActionResult> DeactivateItem(
        long itemId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateServiceItemCommand(itemId), cancellationToken);
        return HandleFailure(result);
    }
}
