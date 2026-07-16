namespace Parking.Application.Features.Customer.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllCustomersByBranchQuery(long BranchId) : IQuery<List<CustomerDto>>;
