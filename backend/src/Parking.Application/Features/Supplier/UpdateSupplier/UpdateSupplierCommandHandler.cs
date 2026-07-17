namespace Parking.Application.Features.Supplier.UpdateSupplier;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class UpdateSupplierCommandHandler : ICommandHandler<UpdateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return Result.Failure<SupplierDto>(new Error("Supplier.NotFound", "Supplier not found."));

        var updateResult = supplier.Update(request.Name, request.Document, request.Phone, request.Email);
        if (updateResult.IsFailure)
            return Result.Failure<SupplierDto>(updateResult.Error);

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new SupplierDto(
            supplier.Id, supplier.BranchId, supplier.Name, supplier.Document,
            supplier.Phone, supplier.Email, supplier.IsActive));
    }
}
