namespace Parking.Domain.Entities;

using Parking.Domain.Common;

public sealed class Vehicle : AggregateRoot
{
    public long CustomerId { get; private set; }
    public string LicensePlate { get; private set; } = null!;
    public string? Model { get; private set; }
    public string? Color { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Vehicle() { }

    private Vehicle(
        long customerId,
        string licensePlate,
        string? model,
        string? color) : base(0)
    {
        CustomerId = customerId;
        LicensePlate = licensePlate;
        Model = model;
        Color = color;
    }

    public static Result<Vehicle> Create(
        long customerId,
        string licensePlate,
        string? model,
        string? color)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return Result.Failure<Vehicle>(new Error("Vehicle.InvalidPlate", "License plate required."));

        return Result.Success(new Vehicle(customerId, licensePlate.Trim().ToUpperInvariant(), model, color));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
