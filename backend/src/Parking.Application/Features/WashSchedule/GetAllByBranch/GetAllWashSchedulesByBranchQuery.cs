namespace Parking.Application.Features.WashSchedule.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllWashSchedulesByBranchQuery(long BranchId) : IQuery<List<WashScheduleDto>>;
