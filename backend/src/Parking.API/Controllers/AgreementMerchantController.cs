namespace Parking.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Parking.Application.Features.AgreementMerchant.CreateAgreementMerchant;
using Parking.Application.Features.AgreementMerchant.DeactivateAgreementMerchant;
using Parking.Application.Features.AgreementMerchant.GetAllByBranch;
using Parking.Application.Features.AgreementMerchant.UpdateAgreementMerchant;

[Route("api/agreement-merchant")]
public sealed class AgreementMerchantController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAgreementMerchantCommand command,
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
        var result = await Mediator.Send(new GetAllAgreementMerchantsByBranchQuery(branchId), cancellationToken);
        return HandleFailure(result);
    }

    [HttpPut("{agreementMerchantId}")]
    public async Task<IActionResult> Update(
        long agreementMerchantId,
        UpdateAgreementMerchantCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command with { AgreementMerchantId = agreementMerchantId }, cancellationToken);
        return HandleFailure(result);
    }

    [HttpPost("{agreementMerchantId}/deactivate")]
    public async Task<IActionResult> Deactivate(
        long agreementMerchantId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateAgreementMerchantCommand(agreementMerchantId), cancellationToken);
        return HandleFailure(result);
    }
}
