namespace Parking.Application.Features.CashRegister.OpenCashRegister;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class OpenCashRegisterCommandHandler : ICommandHandler<OpenCashRegisterCommand, CashRegisterDto>
{
    private readonly ICashRegisterRepository _cashRegisterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenCashRegisterCommandHandler(
        ICashRegisterRepository cashRegisterRepository,
        IUnitOfWork unitOfWork)
    {
        _cashRegisterRepository = cashRegisterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CashRegisterDto>> Handle(OpenCashRegisterCommand request, CancellationToken cancellationToken)
    {
        var existingOpen = await _cashRegisterRepository.GetOpenByBranchAsync(request.BranchId, cancellationToken);
        if (existingOpen is not null)
            return Result.Failure<CashRegisterDto>(
                new Error("CashRegister.AlreadyOpen", "A cash register is already open for this branch."));

        var registerResult = CashRegister.Create(
            request.BranchId,
            request.EmployeeId,
            request.OpeningBalance);

        if (registerResult.IsFailure)
            return Result.Failure<CashRegisterDto>(registerResult.Error);

        await _cashRegisterRepository.AddAsync(registerResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CashRegisterDto(
            registerResult.Value.Id,
            registerResult.Value.BranchId,
            registerResult.Value.EmployeeId,
            registerResult.Value.OpenedAt,
            registerResult.Value.ClosedAt,
            registerResult.Value.OpeningBalance,
            registerResult.Value.ClosingBalance,
            registerResult.Value.Status,
            registerResult.Value.IsActive));
    }
}
