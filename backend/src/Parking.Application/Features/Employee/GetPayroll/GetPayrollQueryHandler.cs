namespace Parking.Application.Features.Employee.GetPayroll;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetPayrollQueryHandler : IQueryHandler<GetPayrollQuery, List<EmployeePayrollDto>>
{
    private readonly IEmployeePayrollRepository _payrollRepository;

    public GetPayrollQueryHandler(IEmployeePayrollRepository payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    public async Task<Result<List<EmployeePayrollDto>>> Handle(GetPayrollQuery request, CancellationToken cancellationToken)
    {
        var payrolls = await _payrollRepository.GetByEmployeeAsync(request.EmployeeId, cancellationToken);

        var dtos = payrolls.Select(p => new EmployeePayrollDto(
            p.Id,
            p.EmployeeId,
            p.MonthYear,
            p.BaseSalary,
            p.Bonuses,
            p.Deductions,
            p.GetTotalAmount(),
            p.Status,
            p.PaidDate,
            p.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
