namespace Parking.Application.Features.Employee.GetById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetEmployeeByIdQuery(long EmployeeId) : IQuery<EmployeeDto>;
