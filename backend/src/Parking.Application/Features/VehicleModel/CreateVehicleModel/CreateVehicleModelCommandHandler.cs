namespace Parking.Application.Features.VehicleModel.CreateVehicleModel;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainVehicleModel = Parking.Domain.Entities.VehicleModel;

internal sealed class CreateVehicleModelCommandHandler : ICommandHandler<CreateVehicleModelCommand, VehicleModelDto>
{
    private readonly IVehicleModelRepository _vehicleModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleModelCommandHandler(
        IVehicleModelRepository vehicleModelRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleModelRepository = vehicleModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VehicleModelDto>> Handle(CreateVehicleModelCommand request, CancellationToken cancellationToken)
    {
        // Get-or-create: se o modelo ja existir no catalogo (ignorando maiusculas/minusculas),
        // devolve o existente em vez de duplicar. Isso mantem o catalogo limpo mesmo se dois
        // funcionarios tentarem "cadastrar" o mesmo modelo novo ao mesmo tempo.
        var existing = await _vehicleModelRepository.GetByNameAsync(request.Name.Trim(), cancellationToken);
        if (existing is not null)
            return Result.Success(new VehicleModelDto(existing.Id, existing.Name, existing.IsActive));

        var modelResult = DomainVehicleModel.Create(request.Name);
        if (modelResult.IsFailure)
            return Result.Failure<VehicleModelDto>(modelResult.Error);

        await _vehicleModelRepository.AddAsync(modelResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new VehicleModelDto(
            modelResult.Value.Id,
            modelResult.Value.Name,
            modelResult.Value.IsActive));
    }
}
