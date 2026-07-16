namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class WashOperationalCost : AggregateRoot
{
    public long BranchId { get; private set; }
    public DateTime MonthYear { get; private set; }
    public decimal LaborCost { get; private set; }
    public decimal MaterialCost { get; private set; }
    public decimal EquipmentCost { get; private set; }
    public decimal UtilitiesCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal TotalRevenue { get; private set; }
    public bool IsActive { get; private set; } = true;

    private WashOperationalCost() { }

    private WashOperationalCost(
        long branchId,
        DateTime monthYear,
        decimal laborCost,
        decimal materialCost,
        decimal equipmentCost,
        decimal utilitiesCost,
        decimal totalRevenue) : base(0)
    {
        BranchId = branchId;
        MonthYear = monthYear;
        LaborCost = laborCost;
        MaterialCost = materialCost;
        EquipmentCost = equipmentCost;
        UtilitiesCost = utilitiesCost;
        TotalCost = laborCost + materialCost + equipmentCost + utilitiesCost;
        TotalRevenue = totalRevenue;
    }

    public static Result<WashOperationalCost> Create(
        long branchId,
        DateTime monthYear,
        decimal laborCost,
        decimal materialCost,
        decimal equipmentCost,
        decimal utilitiesCost,
        decimal totalRevenue)
    {
        if (branchId <= 0)
            return Result.Failure<WashOperationalCost>(new Error("WashOperationalCost.InvalidBranch", "Branch is required."));

        if (laborCost < 0 || materialCost < 0 || equipmentCost < 0 || utilitiesCost < 0)
            return Result.Failure<WashOperationalCost>(new Error("WashOperationalCost.InvalidCost", "Costs cannot be negative."));

        if (totalRevenue < 0)
            return Result.Failure<WashOperationalCost>(new Error("WashOperationalCost.InvalidRevenue", "Revenue cannot be negative."));

        return Result.Success(new WashOperationalCost(
            branchId, monthYear, laborCost, materialCost, equipmentCost, utilitiesCost, totalRevenue));
    }

    public decimal NetProfit => TotalRevenue - TotalCost;

    public decimal ProfitMargin => TotalRevenue == 0 ? 0 : Math.Round((NetProfit / TotalRevenue) * 100, 2);
}
