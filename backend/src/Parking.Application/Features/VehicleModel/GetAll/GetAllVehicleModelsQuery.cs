namespace Parking.Application.Features.VehicleModel.GetAll;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;

public sealed record GetAllVehicleModelsQuery : IQuery<List<VehicleModelDto>>;
