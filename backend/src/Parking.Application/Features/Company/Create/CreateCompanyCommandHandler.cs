namespace Parking.Application.Features.Company.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate CNPJ
        var existing = await _companyRepository.GetByCnpjAsync(request.Cnpj, cancellationToken);
        if (existing is not null)
            return Result.Failure<CompanyDto>(
                new Error("Company.DuplicateCnpj", "Company with this CNPJ already exists."));

        var companyResult = Company.Create(request.Name, request.Cnpj, request.LegalName);
        if (companyResult.IsFailure)
            return Result.Failure<CompanyDto>(companyResult.Error);

        await _companyRepository.AddAsync(companyResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CompanyDto(
            companyResult.Value.Id,
            companyResult.Value.Name,
            companyResult.Value.Cnpj,
            companyResult.Value.LegalName,
            companyResult.Value.IsActive));
    }
}
