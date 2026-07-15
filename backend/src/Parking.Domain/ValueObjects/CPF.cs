namespace Parking.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Parking.Domain.Common;

public sealed class CPF : ValueObject
{
    public string Value { get; }

    private static readonly Regex CPFRegex = new(@"^\d{11}$", RegexOptions.Compiled);

    private CPF(string value) => Value = value;

    public static Result<CPF> Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || !CPFRegex.IsMatch(cpf))
            return Result.Failure<CPF>(new Error("CPF.Invalid", "CPF must be 11 digits."));

        return Result.Success(new CPF(cpf.Trim()));
    }

    public override IEnumerable<object> GetAtomicValues() { yield return Value; }
}
