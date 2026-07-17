namespace Parking.Application.Features.Employee.TerminateEmployee;

using Parking.Application.Abstractions.Messaging;

public sealed record TerminateEmployeeCommand(long EmployeeId) : ICommand;
