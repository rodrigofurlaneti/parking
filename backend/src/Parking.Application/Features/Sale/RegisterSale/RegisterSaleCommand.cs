namespace Parking.Application.Features.Sale.RegisterSale;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record PaymentInput(int PaymentMethod, decimal Amount);

public sealed record RegisterSaleCommand(
    long BranchId,
    long VehicleExitId,
    long CashRegisterId,
    List<PaymentInput> Payments) : ICommand<SaleDto>;
