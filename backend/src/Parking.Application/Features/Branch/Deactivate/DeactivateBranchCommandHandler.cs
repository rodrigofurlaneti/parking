namespace Parking.Application.Features.Branch.Deactivate;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class DeactivateBranchCommandHandler : ICommandHandler<DeactivateBranchCommand>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateBranchCommandHandler(IBranchRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result.Failure(new Error("Branch.NotFound", "Branch not found."));

        branch.Deactivate();
        await _repository.UpdateAsync(branch, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
