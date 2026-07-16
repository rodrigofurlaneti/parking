namespace Parking.Application.Features.WashOperationalCost.GetMonthly;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetMonthlyMetricsQuery(long BranchId, DateTime MonthYear) : IQuery<WashOperationalCostDto>;
