namespace Parking.Application.Features.MonthlyCustomerContract.CreateMonthlyContract;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainMonthlyCustomerContract = Parking.Domain.Entities.MonthlyCustomerContract;

internal sealed class CreateMonthlyContractCommandHandler : ICommandHandler<CreateMonthlyContractCommand, MonthlyCustomerContractDto>
{
    private readonly IMonthlyCustomerContractRepository _contractRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMonthlyContractCommandHandler(
        IMonthlyCustomerContractRepository contractRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MonthlyCustomerContractDto>> Handle(CreateMonthlyContractCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<MonthlyCustomerContractDto>(new Error("Customer.NotFound", "Customer not found."));

        var contractResult = DomainMonthlyCustomerContract.Create(
            request.CustomerId,
            request.BranchId,
            request.MonthlyFee,
            request.MaxVehicles,
            request.StartDate,
            request.EndDate);

        if (contractResult.IsFailure)
            return Result.Failure<MonthlyCustomerContractDto>(contractResult.Error);

        var contract = contractResult.Value;
        await _contractRepository.AddAsync(contract, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new MonthlyCustomerContractDto(
            contract.Id,
            contract.CustomerId,
            contract.BranchId,
            contract.MonthlyFee,
            contract.MaxVehicles,
            contract.StartDate,
            contract.EndDate,
            contract.IsActive));
    }
}
