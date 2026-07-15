namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.VehicleEntry.GetVehicleEntry;
using Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;
using Parking.Application.Features.VehicleEntry.RegisterVehicleExit;

[Route("api/[controller]")]
public sealed class VehicleEntryController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("entry")]
    public async Task<IActionResult> RegisterEntry(
        RegisterVehicleEntryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("exit")]
    public async Task<IActionResult> RegisterExit(
        RegisterVehicleExitCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{vehicleEntryId}")]
    public async Task<IActionResult> GetEntry(
        long vehicleEntryId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetVehicleEntryQuery(vehicleEntryId), cancellationToken);
        return HandleFailure(result);
    }
}
