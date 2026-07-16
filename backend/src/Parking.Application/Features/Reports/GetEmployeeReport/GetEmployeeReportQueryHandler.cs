namespace Parking.Application.Features.Reports.GetEmployeeReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetEmployeeReportQueryHandler : IQueryHandler<GetEmployeeReportQuery, EmployeeReportDto>
{
    private readonly IReportsRepository _reportsRepository;

    public GetEmployeeReportQueryHandler(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<EmployeeReportDto>> Handle(GetEmployeeReportQuery request, CancellationToken cancellationToken)
    {
        var employees = await _reportsRepository.GetActiveEmployeesAsync(request.BranchId, cancellationToken);
        var employeeIds = employees.Select(e => e.Id).ToList();

        var schedules = await _reportsRepository.GetEmployeeSchedulesAsync(employeeIds, cancellationToken);
        var washSessionCounts = await _reportsRepository.GetCompletedWashSessionCountsByEmployeeAsync(
            request.BranchId, request.FromDate, request.ToDate, cancellationToken);

        var washSessionCountsByEmployee = washSessionCounts.ToDictionary(x => x.EmployeeId, x => x.Count);

        // Count how many times each weekday (Monday=0 .. Sunday=6, matching EmployeeSchedule.DayOfWeek) occurs in the period.
        var dayOccurrences = new int[7];
        for (var day = request.FromDate.Date; day <= request.ToDate.Date; day = day.AddDays(1))
        {
            var index = ((int)day.DayOfWeek + 6) % 7;
            dayOccurrences[index]++;
        }

        var schedulesByEmployee = schedules
            .GroupBy(s => s.EmployeeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var employeeProductivity = employees
            .Select(employee =>
            {
                var hoursWorked = 0m;
                if (schedulesByEmployee.TryGetValue(employee.Id, out var employeeSchedules))
                {
                    hoursWorked = employeeSchedules.Sum(s =>
                        (decimal)(s.EndTime - s.StartTime).TotalHours * dayOccurrences[s.DayOfWeek]);
                }

                var washSessionsCompleted = washSessionCountsByEmployee.TryGetValue(employee.Id, out var count)
                    ? count
                    : 0;

                return new EmployeeProductivityDto(employee.Id, employee.Name, hoursWorked, washSessionsCompleted);
            })
            .ToList();

        var dto = new EmployeeReportDto(request.BranchId, request.FromDate, request.ToDate, employeeProductivity);

        return Result.Success(dto);
    }
}
