namespace Parking.Application.Features.Common.DTOs;

public sealed record AgreementCustomerContractDto(
    long Id,
    long CustomerId,
    long AgreementMerchantId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive);
