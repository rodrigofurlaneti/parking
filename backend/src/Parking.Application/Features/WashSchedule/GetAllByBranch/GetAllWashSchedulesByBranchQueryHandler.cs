namespace Parking.Application.Features.WashSchedule.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllWashSchedulesByBranchQueryHandler : IQueryHandler<GetAllWashSchedulesByBranchQuery, List<WashScheduleDto>>
{
    private readonly IWashScheduleRepository _washScheduleRepository;

    public GetAllWashSchedulesByBranchQueryHandler(IWashScheduleRepository washScheduleRepository)
    {
        _washScheduleRepository = washScheduleRepository;
    }

    public async Task<Result<List<WashScheduleDto>>> Handle(GetAllWashSchedulesByBranchQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _washScheduleRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = schedules.Select(x => new WashScheduleDto(
            x.Id,
            x.BranchId,
            x.VehicleEntryId,
            x.ScheduledTime,
            x.ActualStartTime,
            x.ActualEndTime,
            x.EmployeeId,
            x.Status,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
