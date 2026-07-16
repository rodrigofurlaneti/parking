namespace Parking.Application.Features.WashSchedule.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainWashSchedule = Parking.Domain.Entities.WashSchedule;

internal sealed class CreateWashScheduleCommandHandler : ICommandHandler<CreateWashScheduleCommand, WashScheduleDto>
{
    private readonly IWashScheduleRepository _washScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWashScheduleCommandHandler(
        IWashScheduleRepository washScheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _washScheduleRepository = washScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WashScheduleDto>> Handle(CreateWashScheduleCommand request, CancellationToken cancellationToken)
    {
        var scheduleResult = DomainWashSchedule.Create(
            request.BranchId,
            request.VehicleEntryId,
            request.ScheduledTime,
            request.EmployeeId);

        if (scheduleResult.IsFailure)
            return Result.Failure<WashScheduleDto>(scheduleResult.Error);

        await _washScheduleRepository.AddAsync(scheduleResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new WashScheduleDto(
            scheduleResult.Value.Id,
            scheduleResult.Value.BranchId,
            scheduleResult.Value.VehicleEntryId,
            scheduleResult.Value.ScheduledTime,
            scheduleResult.Value.ActualStartTime,
            scheduleResult.Value.ActualEndTime,
            scheduleResult.Value.EmployeeId,
            scheduleResult.Value.Status,
            scheduleResult.Value.IsActive));
    }
}
