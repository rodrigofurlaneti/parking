namespace Parking.Application.Features.MonthlyCustomerContract.CreateMonthlyContract;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateMonthlyContractCommand(
    long CustomerId,
    long BranchId,
    decimal MonthlyFee,
    int MaxVehicles,
    DateTime StartDate,
    DateTime EndDate) : ICommand<MonthlyCustomerContractDto>;
