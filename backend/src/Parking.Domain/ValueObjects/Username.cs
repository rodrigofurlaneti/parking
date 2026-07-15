namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class Username : ValueObject
{
    public string Value { get; }

    private static readonly Regex UsernameRegex = new(
        @"^[a-z0-9._]+$",
        RegexOptions.Compiled);

    private Username(string value) => Value = value;

    public static Result<Username> Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 80)
            return Result.Failure<Username>(
                new Error("Username.InvalidLength", "Username must be between 3 and 80 characters."));

        if (!UsernameRegex.IsMatch(username.ToLowerInvariant()))
            return Result.Failure<Username>(
                new Error("Username.InvalidFormat", "Username may only contain lowercase letters, digits, dots, and underscores."));

        return Result.Success(new Username(username.Trim().ToLowerInvariant()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}
