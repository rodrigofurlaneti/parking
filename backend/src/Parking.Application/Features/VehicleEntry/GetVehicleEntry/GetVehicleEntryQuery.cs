namespace Parking.Application.Features.VehicleEntry.GetVehicleEntry;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetVehicleEntryQuery(long VehicleEntryId) : IQuery<VehicleEntryDto>;
