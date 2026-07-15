namespace Parking.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetAtomicValues();

    public bool Equals(ValueObject? other)
    {
        if (other is null)
            return false;

        if (GetType() != other.GetType())
            return false;

        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override bool Equals(object? obj) => obj is ValueObject valueObject && Equals(valueObject);
    public override int GetHashCode() => GetAtomicValues()
        .Aggregate(default(int), (hashcode, value) =>
            HashCode.Combine(hashcode, value?.GetHashCode()));

    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);
    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
