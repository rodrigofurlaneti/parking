namespace Parking.Application.Features.AgreementMerchant.DeactivateAgreementMerchant;

using Parking.Application.Abstractions.Messaging;

public sealed record DeactivateAgreementMerchantCommand(long AgreementMerchantId) : ICommand;
