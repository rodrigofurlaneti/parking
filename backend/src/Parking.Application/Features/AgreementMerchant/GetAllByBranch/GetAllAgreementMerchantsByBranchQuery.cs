namespace Parking.Application.Features.AgreementMerchant.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllAgreementMerchantsByBranchQuery(long BranchId) : IQuery<List<AgreementMerchantDto>>;
