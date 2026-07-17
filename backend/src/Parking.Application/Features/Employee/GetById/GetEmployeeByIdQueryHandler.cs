namespace Parking.Application.Features.Employee.GetById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<EmployeeDto>(new Error("Employee.NotFound", "Employee not found."));

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
