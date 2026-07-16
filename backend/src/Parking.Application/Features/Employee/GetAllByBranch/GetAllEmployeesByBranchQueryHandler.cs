namespace Parking.Application.Features.Employee.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllEmployeesByBranchQueryHandler : IQueryHandler<GetAllEmployeesByBranchQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetAllEmployeesByBranchQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<List<EmployeeDto>>> Handle(GetAllEmployeesByBranchQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = employees.Select(x => new EmployeeDto(
            x.Id,
            x.CompanyId,
            x.BranchId,
            x.Name,
            x.Email,
            x.Phone,
            x.CPF,
            x.HireDate,
            x.TerminationDate,
            x.RoleId,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
