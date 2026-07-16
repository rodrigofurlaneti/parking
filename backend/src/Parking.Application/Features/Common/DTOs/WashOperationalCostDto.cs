namespace Parking.Application.Features.Common.DTOs;

public sealed record WashOperationalCostDto(
    long Id,
    long BranchId,
    DateTime MonthYear,
    decimal LaborCost,
    decimal MaterialCost,
    decimal EquipmentCost,
    decimal UtilitiesCost,
    decimal TotalCost,
    decimal TotalRevenue,
    decimal NetProfit,
    decimal ProfitMargin,
    bool IsActive);
