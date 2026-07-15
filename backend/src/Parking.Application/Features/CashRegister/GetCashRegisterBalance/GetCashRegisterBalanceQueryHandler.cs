namespace Parking.Application.Features.CashRegister.GetCashRegisterBalance;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class GetCashRegisterBalanceQueryHandler : IQueryHandler<GetCashRegisterBalanceQuery, CashRegisterBalanceDto>
{
    private readonly ICashRegisterRepository _registerRepository;
    private readonly ICashMovementRepository _movementRepository;

    public GetCashRegisterBalanceQueryHandler(
        ICashRegisterRepository registerRepository,
        ICashMovementRepository movementRepository)
    {
        _registerRepository = registerRepository;
        _movementRepository = movementRepository;
    }

    public async Task<Result<CashRegisterBalanceDto>> Handle(GetCashRegisterBalanceQuery request, CancellationToken cancellationToken)
    {
        var register = await _registerRepository.GetByIdAsync(request.CashRegisterId, cancellationToken);
        if (register is null)
            return Result.Failure<CashRegisterBalanceDto>(
                new Error("CashRegister.NotFound", "Cash register not found."));

        var totalMovements = await _movementRepository.GetTotalByRegisterAsync(request.CashRegisterId, cancellationToken);
        var calculatedBalance = register.OpeningBalance + totalMovements;

        return Result.Success(new CashRegisterBalanceDto(
            register.Id,
            register.OpeningBalance,
            totalMovements,
            calculatedBalance));
    }
}
