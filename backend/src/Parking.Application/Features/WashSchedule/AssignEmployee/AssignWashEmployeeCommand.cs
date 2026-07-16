namespace Parking.Application.Features.WashSchedule.AssignEmployee;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record AssignWashEmployeeCommand(
    long WashScheduleId,
    long EmployeeId) : ICommand<WashScheduleDto>;
