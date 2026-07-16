namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class MonthlyCustomerContract : Entity
{
    public long CustomerId { get; private set; }
    public long BranchId { get; private set; }
    public decimal MonthlyFee { get; private set; }
    public int MaxVehicles { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private MonthlyCustomerContract() { }

    private MonthlyCustomerContract(
        long customerId,
        long branchId,
        decimal monthlyFee,
        int maxVehicles,
        DateTime startDate,
        DateTime endDate) : base(0)
    {
        CustomerId = customerId;
        BranchId = branchId;
        MonthlyFee = monthlyFee;
        MaxVehicles = maxVehicles;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Result<MonthlyCustomerContract> Create(
        long customerId,
        long branchId,
        decimal monthlyFee,
        int maxVehicles,
        DateTime startDate,
        DateTime endDate)
    {
        if (monthlyFee <= 0)
            return Result.Failure<MonthlyCustomerContract>(
                new Error("MonthlyCustomerContract.InvalidFee", "Monthly fee must be greater than 0."));

        if (maxVehicles <= 0)
            return Result.Failure<MonthlyCustomerContract>(
                new Error("MonthlyCustomerContract.InvalidMaxVehicles", "Max vehicles must be greater than 0."));

        if (endDate <= startDate)
            return Result.Failure<MonthlyCustomerContract>(
                new Error("MonthlyCustomerContract.InvalidPeriod", "End date must be after start date."));

        return Result.Success(new MonthlyCustomerContract(customerId, branchId, monthlyFee, maxVehicles, startDate, endDate));
    }

    public bool IsValidOn(DateTime date)
    {
        return IsActive && StartDate <= date && date <= EndDate;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
