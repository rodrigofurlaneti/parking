namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.VehicleEntry.GetOpenByBranch;
using Parking.Application.Features.VehicleEntry.GetVehicleEntry;
using Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;
using Parking.Application.Features.VehicleEntry.RegisterVehicleEntryByPlate;
using Parking.Application.Features.VehicleEntry.RegisterVehicleExit;

[Route("api/vehicle-entry")]
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

    // Entrada rapida: so placa + vaga. Resolve/cria o cliente automaticamente.
    [HttpPost("entry-by-plate")]
    public async Task<IActionResult> RegisterEntryByPlate(
        RegisterVehicleEntryByPlateCommand command,
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

    [HttpGet("open")]
    public async Task<IActionResult> GetOpenEntries(
        [FromQuery] long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOpenVehicleEntriesByBranchQuery(branchId), cancellationToken);
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
