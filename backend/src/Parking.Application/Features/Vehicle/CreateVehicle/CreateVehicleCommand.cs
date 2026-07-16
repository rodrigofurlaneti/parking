namespace Parking.Application.Features.Vehicle.CreateVehicle;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateVehicleCommand(
    long CustomerId,
    string LicensePlate,
    string? Model,
    string? Color) : ICommand<VehicleDto>;
