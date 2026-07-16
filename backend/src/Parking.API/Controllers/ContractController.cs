namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.AgreementCustomerContract.CreateAgreementContract;
using Parking.Application.Features.MonthlyCustomerContract.CreateMonthlyContract;

[Route("api/contract")]
public sealed class ContractController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("agreement")]
    public async Task<IActionResult> CreateAgreement(
        CreateAgreementContractCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("monthly")]
    public async Task<IActionResult> CreateMonthly(
        CreateMonthlyContractCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleFailure(result);
    }
}
