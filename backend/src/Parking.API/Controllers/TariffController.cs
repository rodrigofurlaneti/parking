namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Tariff.CreateTariff;
using Parking.Application.Features.Tariff.DeactivateTariff;
using Parking.Application.Features.Tariff.GetAllByBranch;
using Parking.Application.Features.Tariff.UpdateTariff;

[Route("api/[controller]")]
public sealed class TariffController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTariffCommand command,
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
        var result = await Mediator.Send(new GetAllTariffsByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpPut("{tariffId}")]
    public async Task<IActionResult> Update(
        long tariffId,
        UpdateTariffCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { TariffId = tariffId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{tariffId}/deactivate")]
    public async Task<IActionResult> Deactivate(
        long tariffId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateTariffCommand(tariffId), cancellationToken);
        return HandleFailure(result);
    }
}
