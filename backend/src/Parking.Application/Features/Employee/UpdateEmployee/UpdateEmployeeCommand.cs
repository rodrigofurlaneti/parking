namespace Parking.Application.Features.Employee.UpdateEmployee;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record UpdateEmployeeCommand(
    long EmployeeId,
    string Name,
    string Email,
    string Phone,
    long RoleId) : ICommand<EmployeeDto>;
