namespace Parking.Application.Features.WashSchedule.AssignEmployee;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class AssignWashEmployeeCommandHandler : ICommandHandler<AssignWashEmployeeCommand, WashScheduleDto>
{
    private readonly IWashScheduleRepository _washScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignWashEmployeeCommandHandler(
        IWashScheduleRepository washScheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _washScheduleRepository = washScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WashScheduleDto>> Handle(AssignWashEmployeeCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _washScheduleRepository.GetByIdAsync(request.WashScheduleId, cancellationToken);
        if (schedule is null)
            return Result.Failure<WashScheduleDto>(
                new Error("WashSchedule.NotFound", "Wash schedule not found."));

        var assignResult = schedule.AssignEmployee(request.EmployeeId);
        if (assignResult.IsFailure)
            return Result.Failure<WashScheduleDto>(assignResult.Error);

        await _washScheduleRepository.UpdateAsync(schedule, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

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
