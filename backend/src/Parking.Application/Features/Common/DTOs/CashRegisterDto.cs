namespace Parking.Application.Features.Common.DTOs;

public sealed record CashRegisterDto(
    long Id,
    long BranchId,
    long EmployeeId,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal OpeningBalance,
    decimal ClosingBalance,
    int Status,
    bool IsActive);
