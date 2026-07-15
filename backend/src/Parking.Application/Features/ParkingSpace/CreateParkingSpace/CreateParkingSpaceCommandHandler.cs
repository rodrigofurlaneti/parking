namespace Parking.Application.Features.ParkingSpace.CreateParkingSpace;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CreateParkingSpaceCommandHandler : ICommandHandler<CreateParkingSpaceCommand, ParkingSpaceDto>
{
    private readonly IParkingSpaceRepository _parkingSpaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateParkingSpaceCommandHandler(
        IParkingSpaceRepository parkingSpaceRepository,
        IUnitOfWork unitOfWork)
    {
        _parkingSpaceRepository = parkingSpaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ParkingSpaceDto>> Handle(CreateParkingSpaceCommand request, CancellationToken cancellationToken)
    {
        var existing = await _parkingSpaceRepository.GetByNumberAsync(request.BranchId, request.SpaceNumber, cancellationToken);
        if (existing is not null)
            return Result.Failure<ParkingSpaceDto>(
                new Error("ParkingSpace.DuplicateNumber", "Parking space number already exists in this branch."));

        var spaceResult = ParkingSpace.Create(request.BranchId, request.SpaceNumber, request.Type);

        if (spaceResult.IsFailure)
            return Result.Failure<ParkingSpaceDto>(spaceResult.Error);

        await _parkingSpaceRepository.AddAsync(spaceResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new ParkingSpaceDto(
            spaceResult.Value.Id,
            spaceResult.Value.BranchId,
            spaceResult.Value.SpaceNumber,
            spaceResult.Value.Type,
            spaceResult.Value.Status,
            spaceResult.Value.IsActive));
    }
}
