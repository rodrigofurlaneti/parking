namespace Parking.Application.Features.WashSession.RecordProduct;

using Parking.Application.Abstractions.Messaging;

public sealed record RecordProductConsumptionCommand(
    long WashScheduleId,
    long ProductId,
    decimal QuantityUsed) : ICommand<RecordProductConsumptionResult>;

public sealed record RecordProductConsumptionResult(
    long Id,
    long WashScheduleId,
    long ProductId,
    decimal QuantityUsed,
    decimal UnitCost,
    decimal TotalCost,
    decimal RemainingStock);
