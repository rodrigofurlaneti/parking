namespace Parking.Application.Features.CashRegister.GetOpenByBranch;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetOpenCashRegisterByBranchQueryHandler : IQueryHandler<GetOpenCashRegisterByBranchQuery, CashRegisterDto?>
{
    private readonly ICashRegisterRepository _cashRegisterRepository;

    public GetOpenCashRegisterByBranchQueryHandler(ICashRegisterRepository cashRegisterRepository)
    {
        _cashRegisterRepository = cashRegisterRepository;
    }

    public async Task<Result<CashRegisterDto?>> Handle(GetOpenCashRegisterByBranchQuery request, CancellationToken cancellationToken)
    {
        var cashRegister = await _cashRegisterRepository.GetOpenByBranchAsync(request.BranchId, cancellationToken);

        if (cashRegister is null)
            return Result.Success<CashRegisterDto?>(null);

        var dto = new CashRegisterDto(
            cashRegister.Id,
            cashRegister.BranchId,
            cashRegister.EmployeeId,
            cashRegister.OpenedAt,
            cashRegister.ClosedAt,
            cashRegister.OpeningBalance,
            cashRegister.ClosingBalance,
            cashRegister.Status,
            cashRegister.IsActive);

        return Result.Success<CashRegisterDto?>(dto);
    }
}
