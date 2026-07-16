namespace Parking.Application.Features.Sale.GetSaleById;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetSaleByIdQueryHandler : IQueryHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalePaymentRepository _salePaymentRepository;

    public GetSaleByIdQueryHandler(
        ISaleRepository saleRepository,
        ISalePaymentRepository salePaymentRepository)
    {
        _saleRepository = saleRepository;
        _salePaymentRepository = salePaymentRepository;
    }

    public async Task<Result<SaleDto>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null)
            return Result.Failure<SaleDto>(new Error("Sale.NotFound", "Sale not found."));

        var payments = await _salePaymentRepository.GetBySaleAsync(sale.Id, cancellationToken);

        var paymentDtos = payments
            .Select(p => new SalePaymentDto(p.Id, p.PaymentMethod, p.Amount))
            .ToList();

        return Result.Success(new SaleDto(
            sale.Id,
            sale.BranchId,
            sale.VehicleExitId,
            sale.SaleNumber,
            sale.TotalAmount,
            sale.SaleDate,
            sale.IsActive,
            paymentDtos));
    }
}
