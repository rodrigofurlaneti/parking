namespace Parking.Application.Features.Branch.Create;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class CreateBranchCommandHandler : ICommandHandler<CreateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _branchRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBranchCommandHandler(
        IBranchRepository branchRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BranchDto>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company is null)
            return Result.Failure<BranchDto>(new Error("Company.NotFound", "Company not found."));

        var branchResult = Branch.Create(request.CompanyId, request.Name, request.Address, request.TotalSpaces);
        if (branchResult.IsFailure)
            return Result.Failure<BranchDto>(branchResult.Error);

        await _branchRepository.AddAsync(branchResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new BranchDto(
            branchResult.Value.Id,
            branchResult.Value.CompanyId,
            branchResult.Value.Name,
            branchResult.Value.TotalSpaces,
            branchResult.Value.IsActive));
    }
}
