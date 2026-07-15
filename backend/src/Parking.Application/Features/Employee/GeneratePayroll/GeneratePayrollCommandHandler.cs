namespace Parking.Application.Features.Employee.GeneratePayroll;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class GeneratePayrollCommandHandler : ICommandHandler<GeneratePayrollCommand, long>
{
    private readonly IEmployeePayrollRepository _payrollRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GeneratePayrollCommandHandler(
        IEmployeePayrollRepository payrollRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _payrollRepository = payrollRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(GeneratePayrollCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<long>(new Error("Employee.NotFound", "Employee not found."));

        var payrollResult = EmployeePayroll.Create(
            request.EmployeeId,
            request.MonthYear,
            request.BaseSalary);

        if (payrollResult.IsFailure)
            return Result.Failure<long>(payrollResult.Error);

        await _payrollRepository.AddAsync(payrollResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(payrollResult.Value.Id);
    }
}
