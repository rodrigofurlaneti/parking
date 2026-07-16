namespace Parking.Application.Features.Tariff.CreateTariff;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainTariff = Parking.Domain.Entities.Tariff;

// Cadastro de tarifa por filial. Como so existe uma tarifa ATIVA por filial (ver observacao em
// ITariffRepository), se ja houver uma tarifa ativa para o BranchId informado ela e desativada
// antes de criar a nova - isto e o que da o efeito de "criar ou atualizar" descrito no endpoint.
internal sealed class CreateTariffCommandHandler : ICommandHandler<CreateTariffCommand, TariffDto>
{
    private readonly ITariffRepository _tariffRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTariffCommandHandler(
        ITariffRepository tariffRepository,
        IUnitOfWork unitOfWork)
    {
        _tariffRepository = tariffRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TariffDto>> Handle(CreateTariffCommand request, CancellationToken cancellationToken)
    {
        var tariffResult = DomainTariff.Create(
            request.BranchId,
            request.FirstHourRate,
            request.AdditionalHourRate,
            request.DailyMaxRate);

        if (tariffResult.IsFailure)
            return Result.Failure<TariffDto>(tariffResult.Error);

        var existingActive = await _tariffRepository.GetActiveByBranchAsync(request.BranchId, cancellationToken);
        if (existingActive is not null)
        {
            existingActive.Deactivate();
            await _tariffRepository.UpdateAsync(existingActive, cancellationToken);
        }

        var tariff = tariffResult.Value;
        await _tariffRepository.AddAsync(tariff, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new TariffDto(
            tariff.Id,
            tariff.BranchId,
            tariff.FirstHourRate,
            tariff.AdditionalHourRate,
            tariff.DailyMaxRate,
            tariff.IsActive));
    }
}
