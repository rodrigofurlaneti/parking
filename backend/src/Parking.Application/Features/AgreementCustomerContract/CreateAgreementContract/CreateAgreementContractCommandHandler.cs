namespace Parking.Application.Features.AgreementCustomerContract.CreateAgreementContract;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainAgreementCustomerContract = Parking.Domain.Entities.AgreementCustomerContract;

internal sealed class CreateAgreementContractCommandHandler : ICommandHandler<CreateAgreementContractCommand, AgreementCustomerContractDto>
{
    private readonly IAgreementCustomerContractRepository _contractRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IAgreementMerchantRepository _agreementMerchantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAgreementContractCommandHandler(
        IAgreementCustomerContractRepository contractRepository,
        ICustomerRepository customerRepository,
        IAgreementMerchantRepository agreementMerchantRepository,
        IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _customerRepository = customerRepository;
        _agreementMerchantRepository = agreementMerchantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AgreementCustomerContractDto>> Handle(CreateAgreementContractCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<AgreementCustomerContractDto>(new Error("Customer.NotFound", "Customer not found."));

        var merchant = await _agreementMerchantRepository.GetByIdAsync(request.AgreementMerchantId, cancellationToken);
        if (merchant is null)
            return Result.Failure<AgreementCustomerContractDto>(
                new Error("AgreementMerchant.NotFound", "Agreement merchant not found."));

        var contractResult = DomainAgreementCustomerContract.Create(
            request.CustomerId,
            request.AgreementMerchantId,
            request.StartDate,
            request.EndDate);

        if (contractResult.IsFailure)
            return Result.Failure<AgreementCustomerContractDto>(contractResult.Error);

        var contract = contractResult.Value;
        await _contractRepository.AddAsync(contract, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new AgreementCustomerContractDto(
            contract.Id,
            contract.CustomerId,
            contract.AgreementMerchantId,
            contract.StartDate,
            contract.EndDate,
            contract.IsActive));
    }
}
