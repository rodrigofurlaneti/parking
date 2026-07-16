namespace Parking.Application.Features.ParkingSpace.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllParkingSpacesByBranchQuery(long BranchId) : IQuery<List<ParkingSpaceDto>>;
