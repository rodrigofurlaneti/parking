namespace Parking.Application.Features.Reports.GetEmployeeReport;

using Parking.Application.Abstractions.Messaging;

public sealed record GetEmployeeReportQuery(long BranchId, DateTime FromDate, DateTime ToDate) : IQuery<EmployeeReportDto>;
