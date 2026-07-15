namespace Parking.Application.Features.CashRegister.RecordCashMovement;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record RecordCashMovementCommand(
    long CashRegisterId,
    int Type,
    decimal Amount,
    string Description) : ICommand<CashMovementDto>;
