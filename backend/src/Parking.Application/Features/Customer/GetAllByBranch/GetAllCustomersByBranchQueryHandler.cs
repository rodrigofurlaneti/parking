namespace Parking.Application.Features.Customer.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllCustomersByBranchQueryHandler : IQueryHandler<GetAllCustomersByBranchQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersByBranchQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<List<CustomerDto>>> Handle(GetAllCustomersByBranchQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = customers.Select(x => new CustomerDto(
            x.Id,
            x.BranchId,
            x.CustomerType,
            x.Name,
            x.Document,
            x.Phone,
            x.Email,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
