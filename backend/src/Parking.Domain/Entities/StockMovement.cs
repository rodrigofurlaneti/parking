namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class StockMovement : Entity
{
    public const int CompraEntrada = 1;
    public const int ConsumoSaida = 2;
    public const int AjustePositivo = 3;
    public const int AjusteNegativo = 4;

    public long ProductId { get; private set; }
    public int MovementType { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? ReferencedDocumentType { get; private set; }
    public long? ReferencedDocumentId { get; private set; }
    public DateTime MovementDate { get; private set; }

    private StockMovement() { }

    private StockMovement(
        long productId,
        int movementType,
        decimal quantity,
        decimal unitCost,
        string reason,
        string? referencedDocumentType,
        long? referencedDocumentId) : base(0)
    {
        ProductId = productId;
        MovementType = movementType;
        Quantity = quantity;
        UnitCost = unitCost;
        Reason = reason;
        ReferencedDocumentType = referencedDocumentType;
        ReferencedDocumentId = referencedDocumentId;
        MovementDate = DateTime.UtcNow;
    }

    public static Result<StockMovement> Create(
        long productId,
        int movementType,
        decimal quantity,
        decimal unitCost,
        string reason,
        string? referencedDocumentType = null,
        long? referencedDocumentId = null)
    {
        if (productId <= 0)
            return Result.Failure<StockMovement>(new Error("StockMovement.InvalidProduct", "Product is required."));

        if (movementType < 1 || movementType > 4)
            return Result.Failure<StockMovement>(new Error("StockMovement.InvalidType", "Movement type must be between 1 and 4."));

        if (quantity <= 0)
            return Result.Failure<StockMovement>(new Error("StockMovement.InvalidQuantity", "Quantity must be greater than zero."));

        if (unitCost < 0)
            return Result.Failure<StockMovement>(new Error("StockMovement.InvalidUnitCost", "Unit cost cannot be negative."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure<StockMovement>(new Error("StockMovement.InvalidReason", "Reason is required."));

        return Result.Success(new StockMovement(
            productId, movementType, quantity, unitCost, reason, referencedDocumentType, referencedDocumentId));
    }

    public bool IsInflow() => MovementType == CompraEntrada || MovementType == AjustePositivo;
}
