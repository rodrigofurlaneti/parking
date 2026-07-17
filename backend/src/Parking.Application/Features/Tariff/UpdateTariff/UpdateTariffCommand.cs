namespace Parking.Application.Features.Tariff.UpdateTariff;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateTariffCommand(
    long TariffId,
    decimal FirstHourRate,
    decimal AdditionalHourRate,
    decimal? DailyMaxRate) : ICommand<TariffDto>;
