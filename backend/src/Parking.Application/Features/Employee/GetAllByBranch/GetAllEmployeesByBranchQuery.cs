namespace Parking.Application.Features.Employee.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllEmployeesByBranchQuery(long BranchId) : IQuery<List<EmployeeDto>>;
