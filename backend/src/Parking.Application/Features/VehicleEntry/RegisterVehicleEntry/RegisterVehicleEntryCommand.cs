namespace Parking.Application.Features.VehicleEntry.RegisterVehicleEntry;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record RegisterVehicleEntryCommand(
    long BranchId,
    long ParkingSpaceId,
    long CustomerId,
    string LicensePlate,
    string VehicleModel,
    string VehicleColor) : ICommand<VehicleEntryDto>;
