namespace Parking.Application.Features.Customer.GetCustomerByDocument;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetCustomerByDocumentQueryHandler : IQueryHandler<GetCustomerByDocumentQuery, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByDocumentQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByDocumentQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByDocumentAsync(request.Document, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerDto>(new Error("Customer.NotFound", "Customer not found."));

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
