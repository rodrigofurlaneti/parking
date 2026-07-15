namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class LicensePlate : ValueObject
{
    public string Value { get; }

    private static readonly Regex PlateRegex = new(@"^[A-Z]{3}-\d{4}$", RegexOptions.Compiled);

    private LicensePlate(string value) => Value = value;

    public static Result<LicensePlate> Create(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate) || !PlateRegex.IsMatch(plate))
            return Result.Failure<LicensePlate>(
                new Error("LicensePlate.Invalid", "License plate must be in format ABC-1234."));

        return Result.Success(new LicensePlate(plate.Trim().ToUpperInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}
