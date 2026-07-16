namespace Parking.Application.Features.AgreementMerchant.CreateAgreementMerchant;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateAgreementMerchantCommand(
    long BranchId,
    string CompanyName,
    decimal DiscountPercentage) : ICommand<AgreementMerchantDto>;
