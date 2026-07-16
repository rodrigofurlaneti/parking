namespace Parking.Application.Features.Supplier.GetAllByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetAllSuppliersByBranchQueryHandler : IQueryHandler<GetAllSuppliersByBranchQuery, List<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetAllSuppliersByBranchQueryHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<Result<List<SupplierDto>>> Handle(GetAllSuppliersByBranchQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllByBranchAsync(request.BranchId, cancellationToken);

        var dtos = suppliers.Select(x => new SupplierDto(
            x.Id,
            x.BranchId,
            x.Name,
            x.Document,
            x.Phone,
            x.Email,
            x.IsActive)).ToList();

        return Result.Success(dtos);
    }
}
