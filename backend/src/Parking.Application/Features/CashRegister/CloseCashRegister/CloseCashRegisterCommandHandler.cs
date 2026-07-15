namespace Parking.Application.Features.CashRegister.CloseCashRegister;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Repositories;

internal sealed class CloseCashRegisterCommandHandler : ICommandHandler<CloseCashRegisterCommand, CashRegisterDto>
{
    private readonly ICashRegisterRepository _registerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseCashRegisterCommandHandler(
        ICashRegisterRepository registerRepository,
        IUnitOfWork unitOfWork)
    {
        _registerRepository = registerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CashRegisterDto>> Handle(CloseCashRegisterCommand request, CancellationToken cancellationToken)
    {
        var register = await _registerRepository.GetByIdAsync(request.CashRegisterId, cancellationToken);
        if (register is null)
            return Result.Failure<CashRegisterDto>(new Error("CashRegister.NotFound", "Cash register not found."));

        if (register.Status != 0)
            return Result.Failure<CashRegisterDto>(
                new Error("CashRegister.AlreadyClosed", "Cash register is already closed."));

        register.Close(request.ClosingBalance);

        await _registerRepository.UpdateAsync(register, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CashRegisterDto(
            register.Id,
            register.BranchId,
            register.EmployeeId,
            register.OpenedAt,
            register.ClosedAt,
            register.OpeningBalance,
            register.ClosingBalance,
            register.Status,
            register.IsActive));
    }
}
