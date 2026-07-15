namespace Parking.Application.Features.Employee.CreateEmployee;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateEmployeeCommand(
    long CompanyId,
    long BranchId,
    string Name,
    string Email,
    string Phone,
    string CPF,
    long RoleId) : ICommand<EmployeeDto>;
