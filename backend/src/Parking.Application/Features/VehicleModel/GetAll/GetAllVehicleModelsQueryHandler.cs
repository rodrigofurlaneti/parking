namespace Parking.Application.Features.VehicleModel.GetAll;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllVehicleModelsQueryHandler : IQueryHandler<GetAllVehicleModelsQuery, List<VehicleModelDto>>
{
    private readonly IVehicleModelRepository _vehicleModelRepository;

    public GetAllVehicleModelsQueryHandler(IVehicleModelRepository vehicleModelRepository)
    {
        _vehicleModelRepository = vehicleModelRepository;
    }

    public async Task<Result<List<VehicleModelDto>>> Handle(GetAllVehicleModelsQuery request, CancellationToken cancellationToken)
    {
        var models = await _vehicleModelRepository.GetAllAsync(cancellationToken);
        var dtos = models.Select(x => new VehicleModelDto(x.Id, x.Name, x.IsActive)).ToList();
        return Result.Success(dtos);
    }
}
