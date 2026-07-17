namespace Parking.Application.Features.AgreementMerchant.DeactivateAgreementMerchant;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class DeactivateAgreementMerchantCommandHandler : ICommandHandler<DeactivateAgreementMerchantCommand>
{
    private readonly IAgreementMerchantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateAgreementMerchantCommandHandler(IAgreementMerchantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateAgreementMerchantCommand request, CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdAsync(request.AgreementMerchantId, cancellationToken);
        if (merchant is null)
            return Result.Failure(new Error("AgreementMerchant.NotFound", "Agreement merchant not found."));

        merchant.Deactivate();
        await _repository.UpdateAsync(merchant, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
