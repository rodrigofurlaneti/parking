namespace Parking.Application.Features.Company.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateCompanyCommand(
    string Name,
    string Cnpj,
    string LegalName) : ICommand<CompanyDto>;
