namespace Parking.Domain.ValueObjects;

using Parking.Domain.Common;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure<PhoneNumber>(
                new Error("PhoneNumber.Required", "Phone number is required."));

        var cleaned = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

        if (cleaned.Length < 10 || cleaned.Length > 15)
            return Result.Failure<PhoneNumber>(
                new Error("PhoneNumber.InvalidLength", "Phone number must be between 10 and 15 digits."));

        return Result.Success(new PhoneNumber(phoneNumber.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}
