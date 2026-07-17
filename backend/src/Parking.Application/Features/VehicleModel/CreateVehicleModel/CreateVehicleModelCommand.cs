namespace Parking.Application.Features.VehicleModel.CreateVehicleModel;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record CreateVehicleModelCommand(string Name) : ICommand<VehicleModelDto>;
