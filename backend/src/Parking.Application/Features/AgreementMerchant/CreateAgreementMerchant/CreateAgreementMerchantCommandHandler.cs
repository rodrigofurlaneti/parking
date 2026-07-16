namespace Parking.Application.Features.AgreementMerchant.CreateAgreementMerchant;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainAgreementMerchant = Parking.Domain.Entities.AgreementMerchant;

internal sealed class CreateAgreementMerchantCommandHandler : ICommandHandler<CreateAgreementMerchantCommand, AgreementMerchantDto>
{
    private readonly IAgreementMerchantRepository _agreementMerchantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAgreementMerchantCommandHandler(
        IAgreementMerchantRepository agreementMerchantRepository,
        IUnitOfWork unitOfWork)
    {
        _agreementMerchantRepository = agreementMerchantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AgreementMerchantDto>> Handle(CreateAgreementMerchantCommand request, CancellationToken cancellationToken)
    {
        var merchantResult = DomainAgreementMerchant.Create(
            request.BranchId,
            request.CompanyName,
            request.DiscountPercentage);

        if (merchantResult.IsFailure)
            return Result.Failure<AgreementMerchantDto>(merchantResult.Error);

        var merchant = merchantResult.Value;
        await _agreementMerchantRepository.AddAsync(merchant, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new AgreementMerchantDto(
            merchant.Id,
            merchant.BranchId,
            merchant.CompanyName,
            merchant.DiscountPercentage,
            merchant.IsActive));
    }
}
