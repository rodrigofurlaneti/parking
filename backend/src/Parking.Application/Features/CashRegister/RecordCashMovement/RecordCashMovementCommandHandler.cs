namespace Parking.Application.Features.CashRegister.RecordCashMovement;

using Parking.Application.Abstractions.Messaging;
using Parking.Application.Features.Common.DTOs;
using Parking.Domain.Common;
using Parking.Domain.Entities;
using Parking.Domain.Repositories;

internal sealed class RecordCashMovementCommandHandler : ICommandHandler<RecordCashMovementCommand, CashMovementDto>
{
    private readonly ICashMovementRepository _movementRepository;
    private readonly ICashRegisterRepository _registerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordCashMovementCommandHandler(
        ICashMovementRepository movementRepository,
        ICashRegisterRepository registerRepository,
        IUnitOfWork unitOfWork)
    {
        _movementRepository = movementRepository;
        _registerRepository = registerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CashMovementDto>> Handle(RecordCashMovementCommand request, CancellationToken cancellationToken)
    {
        var register = await _registerRepository.GetByIdAsync(request.CashRegisterId, cancellationToken);
        if (register is null)
            return Result.Failure<CashMovementDto>(new Error("CashRegister.NotFound", "Cash register not found."));

        if (register.Status != 0)
            return Result.Failure<CashMovementDto>(
                new Error("CashRegister.Closed", "Cash register is not open."));

        var movementResult = CashMovement.Create(
            request.CashRegisterId,
            request.Type,
            request.Amount,
            request.Description);

        if (movementResult.IsFailure)
            return Result.Failure<CashMovementDto>(movementResult.Error);

        await _movementRepository.AddAsync(movementResult.Value, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CashMovementDto(
            movementResult.Value.Id,
            movementResult.Value.CashRegisterId,
            movementResult.Value.Type,
            movementResult.Value.Amount,
            movementResult.Value.Description,
            movementResult.Value.ReferencedDocumentType,
            movementResult.Value.ReferencedDocumentId,
            movementResult.Value.IsActive));
    }
}
