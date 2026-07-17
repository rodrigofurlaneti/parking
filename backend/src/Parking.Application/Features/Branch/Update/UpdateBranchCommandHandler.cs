namespace Parking.Application.Features.Branch.Update;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateBranchCommandHandler : ICommandHandler<UpdateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBranchCommandHandler(IBranchRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BranchDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result.Failure<BranchDto>(new Error("Branch.NotFound", "Branch not found."));

        var updateResult = branch.Update(request.Name, request.Address, request.PhoneNumber, request.TotalSpaces);
        if (updateResult.IsFailure)
            return Result.Failure<BranchDto>(updateResult.Error);

        await _repository.UpdateAsync(branch, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new BranchDto(
            branch.Id, branch.CompanyId, branch.Name, branch.TotalSpaces, branch.IsActive));
    }
}
