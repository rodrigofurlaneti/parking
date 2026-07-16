namespace Parking.Application.Features.Customer.CreateCustomer;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainCustomer = Parking.Domain.Entities.Customer;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var documentExists = await _customerRepository.ExistsByDocumentAsync(request.Document, cancellationToken);
        if (documentExists)
            return Result.Failure<CustomerDto>(
                new Error("Customer.DuplicateDocument", "A customer with this document already exists."));

        var customerResult = DomainCustomer.Create(
            request.BranchId,
            request.CustomerType,
            request.Name,
            request.Document,
            request.Phone,
            request.Email);

        if (customerResult.IsFailure)
            return Result.Failure<CustomerDto>(customerResult.Error);

        var customer = customerResult.Value;
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CustomerDto(
            customer.Id,
            customer.BranchId,
            customer.CustomerType,
            customer.Name,
            customer.Document,
            customer.Phone,
            customer.Email,
            customer.IsActive));
    }
}
