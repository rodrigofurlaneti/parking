namespace Parking.Application.Features.Supplier.CreateSupplier;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainSupplier = Parking.Domain.Entities.Supplier;

internal sealed class CreateSupplierCommandHandler : ICommandHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierCommandHandler(
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplierResult = DomainSupplier.Create(
            request.BranchId,
            request.Name,
            request.Document,
            request.Phone,
            request.Email);

        if (supplierResult.IsFailure)
            return Result.Failure<SupplierDto>(supplierResult.Error);

        await _supplierRepository.AddAsync(supplierResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new SupplierDto(
            supplierResult.Value.Id,
            supplierResult.Value.BranchId,
            supplierResult.Value.Name,
            supplierResult.Value.Document,
            supplierResult.Value.Phone,
            supplierResult.Value.Email,
            supplierResult.Value.IsActive));
    }
}
