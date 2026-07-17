namespace Parking.Application.Features.Tariff.DeactivateTariff;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class DeactivateTariffCommandHandler : ICommandHandler<DeactivateTariffCommand>
{
    private readonly ITariffRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateTariffCommandHandler(ITariffRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateTariffCommand request, CancellationToken cancellationToken)
    {
        var tariff = await _repository.GetByIdAsync(request.TariffId, cancellationToken);
        if (tariff is null)
            return Result.Failure(new Error("Tariff.NotFound", "Tariff not found."));

        tariff.Deactivate();
        await _repository.UpdateAsync(tariff, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
