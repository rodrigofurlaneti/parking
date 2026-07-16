namespace Parking.Application.Features.Customer.CreateCustomer;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateCustomerCommand(
    long BranchId,
    int CustomerType,
    string Name,
    string Document,
    string? Phone,
    string? Email) : ICommand<CustomerDto>;
