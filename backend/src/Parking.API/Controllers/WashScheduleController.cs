namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.WashSchedule.AssignEmployee;
using Parking.Application.Features.WashSchedule.Create;
using Parking.Application.Features.WashSchedule.GetAllByBranch;
using Parking.Application.Features.WashSchedule.GetById;

[Route("api/wash-schedules")]
public sealed class WashScheduleController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateWashScheduleCommand command,
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
        var result = await Mediator.Send(new GetAllWashSchedulesByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetWashScheduleQuery(id), cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{id}/assign-employee")]
    public async Task<IActionResult> AssignEmployee(
        long id,
        AssignWashEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { WashScheduleId = id }, cancellationToken);
        return HandleFailure(result);
    }
}
