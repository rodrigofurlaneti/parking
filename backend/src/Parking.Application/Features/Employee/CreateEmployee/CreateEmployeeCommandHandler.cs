namespace Parking.Application.Features.Employee.CreateEmployee;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CreateEmployeeCommandHandler : ICommandHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _employeeRepository.GetByCPFAsync(request.CPF, cancellationToken);
        if (existing is not null)
            return Result.Failure<EmployeeDto>(
                new Error("Employee.DuplicateCPF", "Employee with this CPF already exists."));

        var employeeResult = Employee.Create(
            request.CompanyId,
            request.BranchId,
            request.Name,
            request.Email,
            request.Phone,
            request.CPF,
            request.RoleId);

        if (employeeResult.IsFailure)
            return Result.Failure<EmployeeDto>(employeeResult.Error);

        await _employeeRepository.AddAsync(employeeResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new EmployeeDto(
            employeeResult.Value.Id,
            employeeResult.Value.CompanyId,
            employeeResult.Value.BranchId,
            employeeResult.Value.Name,
            employeeResult.Value.Email,
            employeeResult.Value.Phone,
            employeeResult.Value.CPF,
            employeeResult.Value.HireDate,
            employeeResult.Value.TerminationDate,
            employeeResult.Value.RoleId,
            employeeResult.Value.IsActive));
    }
}
