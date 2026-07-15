namespace Parking.Application.Features.VehicleEntry.RegisterVehicleExit;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record RegisterVehicleExitCommand(
    long VehicleEntryId,
    decimal TotalAmount,
    int ParkingMode) : ICommand<VehicleExitDto>;
