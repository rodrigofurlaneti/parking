namespace Parking.Application.Features.WashSchedule.GetById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetWashScheduleQuery(long Id) : IQuery<WashScheduleDto>;
