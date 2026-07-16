namespace Parking.Application.Features.Reports.GetEmployeeReport;

public sealed record EmployeeReportDto(
    long BranchId,
    DateTime FromDate,
    DateTime ToDate,
    IReadOnlyCollection<EmployeeProductivityDto> Employees);

public sealed record EmployeeProductivityDto(long EmployeeId, string EmployeeName, decimal HoursWorked, int WashSessionsCompleted);
