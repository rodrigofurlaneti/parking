namespace Parking.Application.Features.WashSession.RecordProduct;

using Parking.Application.Abstractions.Messaging;
using Parking.Domain.Common;
using Parking.Domain.Repositories;
using DomainWashProductConsumption = Parking.Domain.Entities.WashProductConsumption;

internal sealed class RecordProductConsumptionCommandHandler
    : ICommandHandler<RecordProductConsumptionCommand, RecordProductConsumptionResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IWashProductConsumptionRepository _consumptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordProductConsumptionCommandHandler(
        IProductRepository productRepository,
        IWashProductConsumptionRepository consumptionRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _consumptionRepository = consumptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RecordProductConsumptionResult>> Handle(
        RecordProductConsumptionCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<RecordProductConsumptionResult>(
                new Error("Product.NotFound", "Product not found."));

        var decreaseResult = product.DecreaseStock(request.QuantityUsed);
        if (decreaseResult.IsFailure)
            return Result.Failure<RecordProductConsumptionResult>(decreaseResult.Error);

        var consumptionResult = DomainWashProductConsumption.Create(
            request.WashScheduleId,
            request.ProductId,
            request.QuantityUsed,
            product.Cost);

        if (consumptionResult.IsFailure)
            return Result.Failure<RecordProductConsumptionResult>(consumptionResult.Error);

        await _consumptionRepository.AddAsync(consumptionResult.Value, cancellationToken);
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RecordProductConsumptionResult(
            consumptionResult.Value.Id,
            consumptionResult.Value.WashScheduleId,
            consumptionResult.Value.ProductId,
            consumptionResult.Value.QuantityUsed,
            consumptionResult.Value.UnitCost,
            consumptionResult.Value.TotalCost,
            product.Stock));
    }
}
