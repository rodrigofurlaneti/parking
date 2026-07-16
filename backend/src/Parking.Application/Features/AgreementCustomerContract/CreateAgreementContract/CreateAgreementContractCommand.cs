namespace Parking.Application.Features.AgreementCustomerContract.CreateAgreementContract;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateAgreementContractCommand(
    long CustomerId,
    long AgreementMerchantId,
    DateTime StartDate,
    DateTime EndDate) : ICommand<AgreementCustomerContractDto>;
