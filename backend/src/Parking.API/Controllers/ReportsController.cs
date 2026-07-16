namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Reports.GetCashReport;
using Parking.Application.Features.Reports.GetEmployeeReport;
using Parking.Application.Features.Reports.GetOccupancyReport;
using Parking.Application.Features.Reports.GetRevenueReport;
using Parking.Application.Features.Reports.GetStockReport;

[Route("api/reports")]
public sealed class ReportsController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy(
        [FromQuery] long branchId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetOccupancyReportQuery(branchId, fromDate, toDate), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(
        [FromQuery] long branchId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetRevenueReportQuery(branchId, fromDate, toDate), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("stock")]
    public async Task<IActionResult> GetStock(
        [FromQuery] long branchId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetStockReportQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] long branchId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetEmployeeReportQuery(branchId, fromDate, toDate), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("cash")]
    public async Task<IActionResult> GetCash(
        [FromQuery] long branchId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCashReportQuery(branchId, fromDate, toDate), cancellationToken);
        return HandleFailure(result);
    }
}
