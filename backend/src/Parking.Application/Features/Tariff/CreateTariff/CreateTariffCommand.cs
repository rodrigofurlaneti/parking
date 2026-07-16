namespace Parking.Application.Features.Tariff.CreateTariff;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateTariffCommand(
    long BranchId,
    decimal FirstHourRate,
    decimal AdditionalHourRate,
    decimal? DailyMaxRate) : ICommand<TariffDto>;
