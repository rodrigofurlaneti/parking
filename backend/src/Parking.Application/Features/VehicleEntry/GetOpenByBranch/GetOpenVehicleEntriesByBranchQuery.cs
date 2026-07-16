namespace Parking.Application.Features.VehicleEntry.GetOpenByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetOpenVehicleEntriesByBranchQuery(long BranchId) : IQuery<List<VehicleEntryDto>>;
