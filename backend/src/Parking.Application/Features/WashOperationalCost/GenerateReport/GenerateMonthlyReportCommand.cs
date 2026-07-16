namespace Parking.Application.Features.WashOperationalCost.GenerateReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GenerateMonthlyReportCommand(
    long BranchId,
    DateTime MonthYear,
    decimal LaborCost,
    decimal MaterialCost,
    decimal EquipmentCost,
    decimal UtilitiesCost,
    decimal TotalRevenue) : ICommand<WashOperationalCostDto>;
