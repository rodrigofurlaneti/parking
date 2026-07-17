namespace Parking.Application.Features.Tariff.UpdateTariff;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateTariffCommandHandler : ICommandHandler<UpdateTariffCommand, TariffDto>
{
    private readonly ITariffRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTariffCommandHandler(ITariffRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TariffDto>> Handle(UpdateTariffCommand request, CancellationToken cancellationToken)
    {
        var tariff = await _repository.GetByIdAsync(request.TariffId, cancellationToken);
        if (tariff is null)
            return Result.Failure<TariffDto>(new Error("Tariff.NotFound", "Tariff not found."));

        var updateResult = tariff.Update(request.FirstHourRate, request.AdditionalHourRate, request.DailyMaxRate);
        if (updateResult.IsFailure)
            return Result.Failure<TariffDto>(updateResult.Error);

        await _repository.UpdateAsync(tariff, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new TariffDto(
            tariff.Id, tariff.BranchId, tariff.FirstHourRate, tariff.AdditionalHourRate, tariff.DailyMaxRate, tariff.IsActive));
    }
}
