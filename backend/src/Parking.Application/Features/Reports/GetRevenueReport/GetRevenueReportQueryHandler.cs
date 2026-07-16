namespace Parking.Application.Features.Reports.GetRevenueReport;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetRevenueReportQueryHandler : IQueryHandler<GetRevenueReportQuery, RevenueReportDto>
{
    private const int CustomerTypeRotativo = 1;
    private const int CustomerTypeConvenio = 2;
    private const int CustomerTypeMensalista = 3;

    private readonly IReportsRepository _reportsRepository;

    public GetRevenueReportQueryHandler(IReportsRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<Result<RevenueReportDto>> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
    {
        var sales = await _reportsRepository.GetSalesWithCustomerTypeAsync(
            request.BranchId, request.FromDate, request.ToDate, cancellationToken);

        var washRevenue = await _reportsRepository.GetWashServiceRevenueTotalAsync(
            request.BranchId, request.FromDate, request.ToDate, cancellationToken);

        var rotativeRevenue = sales.Where(x => x.CustomerType == CustomerTypeRotativo).Sum(x => x.Sale.TotalAmount);
        var agreementRevenue = sales.Where(x => x.CustomerType == CustomerTypeConvenio).Sum(x => x.Sale.TotalAmount);
        var monthlyRevenue = sales.Where(x => x.CustomerType == CustomerTypeMensalista).Sum(x => x.Sale.TotalAmount);

        var parkingTotalRevenue = rotativeRevenue + agreementRevenue + monthlyRevenue;
        var grandTotal = parkingTotalRevenue + washRevenue;

        var dto = new RevenueReportDto(
            request.BranchId,
            request.FromDate,
            request.ToDate,
            rotativeRevenue,
            agreementRevenue,
            monthlyRevenue,
            parkingTotalRevenue,
            washRevenue,
            grandTotal);

        return Result.Success(dto);
    }
}
