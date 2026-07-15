namespace Parking.Application.Features.CashRegister.GetCashRegisterBalance;

using Parking.Application.Abstractions.Messaging;

public sealed record GetCashRegisterBalanceQuery(long CashRegisterId) : IQuery<CashRegisterBalanceDto>;

public sealed record CashRegisterBalanceDto(
    long CashRegisterId,
    decimal OpeningBalance,
    decimal TotalMovements,
    decimal CalculatedBalance);
