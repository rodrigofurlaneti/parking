namespace Parking.Application.Features.WashSchedule.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateWashScheduleCommand(
    long BranchId,
    long VehicleEntryId,
    DateTime ScheduledTime,
    long EmployeeId) : ICommand<WashScheduleDto>;
