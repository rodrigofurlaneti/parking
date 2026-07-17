namespace Parking.Application.Features.Tariff.DeactivateTariff;

using Parking.Application.Abstractions.Messaging;

public sealed record DeactivateTariffCommand(long TariffId) : ICommand;
