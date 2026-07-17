namespace Parking.Application.Features.VehicleModel.Search;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class SearchVehicleModelsQueryHandler : IQueryHandler<SearchVehicleModelsQuery, List<VehicleModelDto>>
{
    private readonly IVehicleModelRepository _vehicleModelRepository;

    public SearchVehicleModelsQueryHandler(IVehicleModelRepository vehicleModelRepository)
    {
        _vehicleModelRepository = vehicleModelRepository;
    }

    public async Task<Result<List<VehicleModelDto>>> Handle(SearchVehicleModelsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return Result.Success(new List<VehicleModelDto>());

        var models = await _vehicleModelRepository.SearchAsync(request.Query.Trim(), 10, cancellationToken);
        var dtos = models.Select(x => new VehicleModelDto(x.Id, x.Name, x.IsActive)).ToList();
        return Result.Success(dtos);
    }
}
