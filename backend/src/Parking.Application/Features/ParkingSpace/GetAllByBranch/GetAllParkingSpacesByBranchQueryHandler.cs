namespace Parking.Application.Features.ParkingSpace.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllParkingSpacesByBranchQueryHandler : IQueryHandler<GetAllParkingSpacesByBranchQuery, List<ParkingSpaceDto>>
{
    private readonly IParkingSpaceRepository _parkingSpaceRepository;

    public GetAllParkingSpacesByBranchQueryHandler(IParkingSpaceRepository parkingSpaceRepository)
    {
        _parkingSpaceRepository = parkingSpaceRepository;
    }

    public async Task<Result<List<ParkingSpaceDto>>> Handle(GetAllParkingSpacesByBranchQuery request, CancellationToken cancellationToken)
    {
        var spaces = await _parkingSpaceRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = spaces.Select(x => new ParkingSpaceDto(
            x.Id,
            x.BranchId,
            x.SpaceNumber,
            x.Type,
            x.Status,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
