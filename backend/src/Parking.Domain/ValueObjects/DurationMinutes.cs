namespace Parking.Domain.ValueObjects;

using Parking.Domain.Common;

public sealed class DurationMinutes : ValueObject
{
    public int Value { get; }

    private DurationMinutes(int value) => Value = value;

    public static Result<DurationMinutes> Create(int minutes)
    {
        if (minutes <= 0)
            return Result.Failure<DurationMinutes>(
                new Error("DurationMinutes.Invalid", "Duration must be greater than 0."));

        return Result.Success(new DurationMinutes(minutes));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}
