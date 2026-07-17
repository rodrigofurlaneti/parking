namespace Parking.Application.Features.AgreementMerchant.UpdateAgreementMerchant;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateAgreementMerchantCommandHandler : ICommandHandler<UpdateAgreementMerchantCommand, AgreementMerchantDto>
{
    private readonly IAgreementMerchantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAgreementMerchantCommandHandler(IAgreementMerchantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AgreementMerchantDto>> Handle(UpdateAgreementMerchantCommand request, CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdAsync(request.AgreementMerchantId, cancellationToken);
        if (merchant is null)
            return Result.Failure<AgreementMerchantDto>(new Error("AgreementMerchant.NotFound", "Agreement merchant not found."));

        var updateResult = merchant.Update(request.CompanyName, request.DiscountPercentage);
        if (updateResult.IsFailure)
            return Result.Failure<AgreementMerchantDto>(updateResult.Error);

        await _repository.UpdateAsync(merchant, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new AgreementMerchantDto(
            merchant.Id, merchant.BranchId, merchant.CompanyName, merchant.DiscountPercentage, merchant.IsActive));
    }
}
