namespace Parking.Application.Features.Common.DTOs;

public sealed record SalePaymentDto(
    long Id,
    int PaymentMethod,
    decimal Amount);
