namespace Parking.Application.Features.Customer.GetCustomerByDocument;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetCustomerByDocumentQuery(string Document) : IQuery<CustomerDto>;
