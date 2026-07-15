namespace Parking.Application.Features.Company.GetById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetCompanyByIdQuery(long CompanyId) : IQuery<CompanyDto>;
