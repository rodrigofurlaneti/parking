namespace Parking.Tests.Unit;

using FluentAssertions;
using Parking.Domain.Entities;
using Xunit;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m, supplierId: 3, minimumStock: 2m);

        result.IsSuccess.Should().BeTrue();
        result.Value.BranchId.Should().Be(1);
        result.Value.Name.Should().Be("Wax");
        result.Value.SKU.Should().Be("SKU1");
        result.Value.Category.Should().Be("Cleaning");
        result.Value.Cost.Should().Be(5m);
        result.Value.SellingPrice.Should().Be(10m);
        result.Value.Stock.Should().Be(20m);
        result.Value.SupplierId.Should().Be(3);
        result.Value.MinimumStock.Should().Be(2m);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidBranch_ShouldFail()
    {
        var result = Product.Create(0, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidBranch");
    }

    [Fact]
    public void Create_WithMissingName_ShouldFail()
    {
        var result = Product.Create(1, "", "SKU1", "Cleaning", 5m, 10m, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidName");
    }

    [Fact]
    public void Create_WithMissingSKU_ShouldFail()
    {
        var result = Product.Create(1, "Wax", "  ", "Cleaning", 5m, 10m, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidSKU");
    }

    [Fact]
    public void Create_WithNegativeCost_ShouldFail()
    {
        var result = Product.Create(1, "Wax", "SKU1", "Cleaning", -1m, 10m, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidCost");
    }

    [Fact]
    public void Create_WithNegativeSellingPrice_ShouldFail()
    {
        var result = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, -1m, 20m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidSellingPrice");
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldFail()
    {
        var result = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidStock");
    }

    [Fact]
    public void Create_WithNegativeMinimumStock_ShouldFail()
    {
        var result = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m, minimumStock: -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidMinimumStock");
    }

    [Fact]
    public void DecreaseStock_WithValidQuantity_ShouldReduceStock()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m).Value;

        var result = product.DecreaseStock(5m);

        result.IsSuccess.Should().BeTrue();
        product.Stock.Should().Be(15m);
    }

    [Fact]
    public void DecreaseStock_WithZeroQuantity_ShouldFail()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m).Value;

        var result = product.DecreaseStock(0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InvalidQuantity");
    }

    [Fact]
    public void DecreaseStock_ExceedingStock_ShouldFail()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m).Value;

        var result = product.DecreaseStock(21m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InsufficientStock");
    }

    [Fact]
    public void IncreaseStock_ShouldAddToStock()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m).Value;

        product.IncreaseStock(5m);

        product.Stock.Should().Be(25m);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 20m).Value;

        product.Deactivate();

        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsBelowMinimum_WhenStockBelowMinimum_ShouldReturnTrue()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 1m, minimumStock: 5m).Value;

        product.IsBelowMinimum().Should().BeTrue();
    }

    [Fact]
    public void IsBelowMinimum_WhenStockAboveMinimum_ShouldReturnFalse()
    {
        var product = Product.Create(1, "Wax", "SKU1", "Cleaning", 5m, 10m, 10m, minimumStock: 5m).Value;

        product.IsBelowMinimum().Should().BeFalse();
    }
}
