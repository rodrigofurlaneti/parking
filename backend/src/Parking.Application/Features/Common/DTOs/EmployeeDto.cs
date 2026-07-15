namespace Parking.Application.Features.Common.DTOs;

public sealed record EmployeeDto(
    long Id,
    long CompanyId,
    long BranchId,
    string Name,
    string Email,
    string Phone,
    string CPF,
    DateTime HireDate,
    DateTime? TerminationDate,
    long RoleId,
    bool IsActive);
