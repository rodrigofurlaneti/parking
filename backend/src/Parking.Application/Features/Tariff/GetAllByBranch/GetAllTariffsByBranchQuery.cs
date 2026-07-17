namespace Parking.Application.Features.Tariff.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllTariffsByBranchQuery(long BranchId) : IQuery<List<TariffDto>>;
