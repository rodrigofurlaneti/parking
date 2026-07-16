namespace Parking.Application.Features.Common.DTOs;

public sealed record AgreementMerchantDto(
    long Id,
    long BranchId,
    string CompanyName,
    decimal DiscountPercentage,
    bool IsActive);
