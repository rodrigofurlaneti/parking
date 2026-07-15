namespace Parking.Application.Features.Employee.GetEmployeeSchedule;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetEmployeeScheduleQueryHandler : IQueryHandler<GetEmployeeScheduleQuery, List<EmployeeScheduleDto>>
{
    private readonly IEmployeeScheduleRepository _scheduleRepository;

    public GetEmployeeScheduleQueryHandler(IEmployeeScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<List<EmployeeScheduleDto>>> Handle(GetEmployeeScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetByEmployeeAsync(request.EmployeeId, cancellationToken);

        var dtos = schedules.Select(s => new EmployeeScheduleDto(
            s.Id,
            s.EmployeeId,
            s.DayOfWeek,
            s.StartTime,
            s.EndTime,
            s.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
