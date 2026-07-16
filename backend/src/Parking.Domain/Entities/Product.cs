namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Product : AggregateRoot
{
    public long BranchId { get; private set; }
    public string Name { get; private set; } = null!;
    public string SKU { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public decimal Cost { get; private set; }
    public decimal SellingPrice { get; private set; }
    public decimal Stock { get; private set; }
    public decimal MinimumStock { get; private set; }
    public long? SupplierId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Product() { }

    private Product(
        long branchId,
        string name,
        string sku,
        string category,
        decimal cost,
        decimal sellingPrice,
        decimal stock,
        long? supplierId,
        decimal minimumStock) : base(0)
    {
        BranchId = branchId;
        Name = name;
        SKU = sku;
        Category = category;
        Cost = cost;
        SellingPrice = sellingPrice;
        Stock = stock;
        SupplierId = supplierId;
        MinimumStock = minimumStock;
    }

    public static Result<Product> Create(
        long branchId,
        string name,
        string sku,
        string category,
        decimal cost,
        decimal sellingPrice,
        decimal stock,
        long? supplierId = null,
        decimal minimumStock = 0)
    {
        if (branchId <= 0)
            return Result.Failure<Product>(new Error("Product.InvalidBranch", "Branch is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Product>(new Error("Product.InvalidName", "Name required."));

        if (string.IsNullOrWhiteSpace(sku))
            return Result.Failure<Product>(new Error("Product.InvalidSKU", "SKU required."));

        if (cost < 0)
            return Result.Failure<Product>(new Error("Product.InvalidCost", "Cost cannot be negative."));

        if (sellingPrice < 0)
            return Result.Failure<Product>(new Error("Product.InvalidSellingPrice", "Selling price cannot be negative."));

        if (stock < 0)
            return Result.Failure<Product>(new Error("Product.InvalidStock", "Stock cannot be negative."));

        if (minimumStock < 0)
            return Result.Failure<Product>(new Error("Product.InvalidMinimumStock", "Minimum stock cannot be negative."));

        return Result.Success(new Product(branchId, name, sku, category, cost, sellingPrice, stock, supplierId, minimumStock));
    }

    public Result DecreaseStock(decimal quantity)
    {
        if (quantity <= 0)
            return Result.Failure(new Error("Product.InvalidQuantity", "Quantity must be greater than zero."));

        if (quantity > Stock)
            return Result.Failure(new Error("Product.InsufficientStock", "Insufficient stock."));

        Stock -= quantity;
        return Result.Success();
    }

    public void IncreaseStock(decimal quantity)
    {
        Stock += quantity;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsBelowMinimum() => Stock < MinimumStock;
}
