namespace Parking.Application.Features.CashRegister.CloseCashRegister;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CloseCashRegisterCommand(
    long CashRegisterId,
    decimal ClosingBalance) : ICommand<CashRegisterDto>;
