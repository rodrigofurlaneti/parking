namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.WashOperationalCost.GenerateReport;
using Parking.Application.Features.WashOperationalCost.GetMonthly;

[Route("api/wash-reports")]
public sealed class WashReportsController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("monthly")]
    public async Task<IActionResult> GenerateMonthly(
        GenerateMonthlyReportCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly(
        [FromQuery] long branchId,
        [FromQuery] DateTime monthYear,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMonthlyMetricsQuery(branchId, monthYear), cancellationToken);
        return HandleFailure(result);
    }
}
