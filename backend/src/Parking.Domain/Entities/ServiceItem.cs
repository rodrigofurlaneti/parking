namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class ServiceItem : AggregateRoot
{
    public long ServiceCategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal BaseCost { get; private set; }
    public bool IsActive { get; private set; } = true;

    private ServiceItem() { }

    private ServiceItem(
        long serviceCategoryId,
        string name,
        string? description,
        int durationMinutes,
        decimal baseCost) : base(0)
    {
        ServiceCategoryId = serviceCategoryId;
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        BaseCost = baseCost;
    }

    public static Result<ServiceItem> Create(
        long serviceCategoryId,
        string name,
        string? description,
        int durationMinutes,
        decimal baseCost)
    {
        if (serviceCategoryId <= 0)
            return Result.Failure<ServiceItem>(new Error("ServiceItem.InvalidCategory", "Service category is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<ServiceItem>(new Error("ServiceItem.InvalidName", "Name required."));

        if (durationMinutes <= 0)
            return Result.Failure<ServiceItem>(new Error("ServiceItem.InvalidDuration", "Duration must be greater than zero."));

        if (baseCost < 0)
            return Result.Failure<ServiceItem>(new Error("ServiceItem.InvalidCost", "Base cost cannot be negative."));

        return Result.Success(new ServiceItem(serviceCategoryId, name, description, durationMinutes, baseCost));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
