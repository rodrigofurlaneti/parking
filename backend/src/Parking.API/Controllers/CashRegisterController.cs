namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.CashRegister.CloseCashRegister;
using Parking.Application.Features.CashRegister.GetCashRegisterBalance;
using Parking.Application.Features.CashRegister.OpenCashRegister;
using Parking.Application.Features.CashRegister.RecordCashMovement;

[Route("api/[controller]")]
public sealed class CashRegisterController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("open")]
    public async Task<IActionResult> Open(
        OpenCashRegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{cashRegisterId}/movements")]
    public async Task<IActionResult> RecordMovement(
        long cashRegisterId,
        RecordCashMovementCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { CashRegisterId = cashRegisterId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{cashRegisterId}/close")]
    public async Task<IActionResult> Close(
        long cashRegisterId,
        CloseCashRegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { CashRegisterId = cashRegisterId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpGet("{cashRegisterId}/balance")]
    public async Task<IActionResult> GetBalance(
        long cashRegisterId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCashRegisterBalanceQuery(cashRegisterId), cancellationToken);
        return HandleFailure(result);
    }
}
