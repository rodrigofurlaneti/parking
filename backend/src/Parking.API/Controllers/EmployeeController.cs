namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.Employee.AssignSchedule;
using Parking.Application.Features.Employee.CreateEmployee;
using Parking.Application.Features.Employee.GeneratePayroll;
using Parking.Application.Features.Employee.GetAllByBranch;
using Parking.Application.Features.Employee.GetEmployeeSchedule;
using Parking.Application.Features.Employee.GetPayroll;

[Route("api/[controller]")]
public sealed class EmployeeController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateEmployeeCommand command,
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
        var result = await Mediator.Send(new GetAllEmployeesByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{employeeId}/schedules")]
    public async Task<IActionResult> AssignSchedule(
        long employeeId,
        AssignScheduleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { EmployeeId = employeeId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{employeeId}/payroll")]
    public async Task<IActionResult> GeneratePayroll(
        long employeeId,
        GeneratePayrollCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { EmployeeId = employeeId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{employeeId}/schedules")]
    public async Task<IActionResult> GetSchedules(
        long employeeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetEmployeeScheduleQuery(employeeId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{employeeId}/payroll")]
    public async Task<IActionResult> GetPayroll(
        long employeeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPayrollQuery(employeeId), cancellationToken);
        return HandleFailure(result);
    }
}
