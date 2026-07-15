namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            return Result.Failure<Email>(
                new Error("Email.Invalid", "Invalid email address."));

        return Result.Success(new Email(email.Trim().ToLowerInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}
