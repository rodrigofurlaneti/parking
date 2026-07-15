namespace Parking.Application.Features.CashRegister.OpenCashRegister;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record OpenCashRegisterCommand(
    long BranchId,
    long EmployeeId,
    decimal OpeningBalance) : ICommand<CashRegisterDto>;
