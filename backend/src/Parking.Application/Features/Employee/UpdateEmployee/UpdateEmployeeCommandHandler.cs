namespace Parking.Application.Features.Employee.UpdateEmployee;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateEmployeeCommandHandler : ICommandHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<EmployeeDto>(new Error("Employee.NotFound", "Employee not found."));

        var updateResult = employee.Update(request.Name, request.Email, request.Phone, request.RoleId);
        if (updateResult.IsFailure)
            return Result.Failure<EmployeeDto>(updateResult.Error);

        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new EmployeeDto(
            employee.Id,
            employee.CompanyId,
            employee.BranchId,
            employee.Name,
            employee.Email,
            employee.Phone,
            employee.CPF,
            employee.HireDate,
            employee.TerminationDate,
            employee.RoleId,
            employee.IsActive));
    }
}
