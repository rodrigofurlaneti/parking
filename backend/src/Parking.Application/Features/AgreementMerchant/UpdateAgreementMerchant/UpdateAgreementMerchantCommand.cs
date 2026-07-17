namespace Parking.Application.Features.AgreementMerchant.UpdateAgreementMerchant;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateAgreementMerchantCommand(
    long AgreementMerchantId,
    string CompanyName,
    decimal DiscountPercentage) : ICommand<AgreementMerchantDto>;
