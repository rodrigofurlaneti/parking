namespace Parking.Application.Features.WashSchedule.GetById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetWashScheduleQueryHandler : IQueryHandler<GetWashScheduleQuery, WashScheduleDto>
{
    private readonly IWashScheduleRepository _washScheduleRepository;

    public GetWashScheduleQueryHandler(IWashScheduleRepository washScheduleRepository)
    {
        _washScheduleRepository = washScheduleRepository;
    }

    public async Task<Result<WashScheduleDto>> Handle(GetWashScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _washScheduleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (schedule is null)
            return Result.Failure<WashScheduleDto>(
                new Error("WashSchedule.NotFound", "Wash schedule not found."));

        return Result.Success(new WashScheduleDto(
            schedule.Id,
            schedule.BranchId,
            schedule.VehicleEntryId,
            schedule.ScheduledTime,
            schedule.ActualStartTime,
            schedule.ActualEndTime,
            schedule.EmployeeId,
            schedule.Status,
            schedule.IsActive));
    }
}
