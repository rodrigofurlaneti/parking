namespace Parking.Application.Features.Common.DTOs;

public sealed record MonthlyCustomerContractDto(
    long Id,
    long CustomerId,
    long BranchId,
    decimal MonthlyFee,
    int MaxVehicles,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive);
