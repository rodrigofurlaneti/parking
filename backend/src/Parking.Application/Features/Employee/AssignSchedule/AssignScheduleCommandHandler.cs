namespace Parking.Application.Features.Employee.AssignSchedule;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class AssignScheduleCommandHandler : ICommandHandler<AssignScheduleCommand, long>
{
    private readonly IEmployeeScheduleRepository _scheduleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignScheduleCommandHandler(
        IEmployeeScheduleRepository scheduleRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(AssignScheduleCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<long>(new Error("Employee.NotFound", "Employee not found."));

        var scheduleResult = EmployeeSchedule.Create(
            request.EmployeeId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime);

        if (scheduleResult.IsFailure)
            return Result.Failure<long>(scheduleResult.Error);

        await _scheduleRepository.AddAsync(scheduleResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(scheduleResult.Value.Id);
    }
}
