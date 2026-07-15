namespace Parking.Application.Features.Common.DTOs;

public sealed record CompanyDto(
    long Id,
    string Name,
    string Cnpj,
    string LegalName,
    bool IsActive);
