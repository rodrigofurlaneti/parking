namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.ParkingSpace.CreateParkingSpace;
using Parking.Application.Features.ParkingSpace.GetAllByBranch;
using Parking.Application.Features.ParkingSpace.GetParkingSpaceDetails;
using Parking.Application.Features.ParkingSpace.GetParkingSpaceOccupancy;

[Route("api/parking-space")]
public sealed class ParkingSpaceController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateParkingSpaceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{parkingSpaceId}")]
    public async Task<IActionResult> GetDetails(
        long parkingSpaceId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetParkingSpaceDetailsQuery(parkingSpaceId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetAllByBranch(
        long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllParkingSpacesByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("branch/{branchId}/occupancy")]
    public async Task<IActionResult> GetOccupancy(
        long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetParkingSpaceOccupancyQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }
}
