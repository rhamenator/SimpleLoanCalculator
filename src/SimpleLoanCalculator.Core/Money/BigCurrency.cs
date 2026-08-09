using System.Text.RegularExpressions;
using SimpleLoanCalculator.Core.Numerics;

namespace SimpleLoanCalculator.Core.Money;

/// <summary>
/// An arbitrary-precision amount paired with a user-defined monetary unit.
/// No assumption is made about a currency symbol or number of minor units.
/// </summary>
public readonly partial record struct BigCurrency
{
    public BigCurrency(BigDecimal amount, string unit)
    {
        unit = unit.Trim();
        if (!ValidUnit().IsMatch(unit))
        {
            throw new ArgumentException(
                "A monetary unit must contain 1-24 letters, numbers, spaces, periods, underscores, or hyphens.",
                nameof(unit));
        }

        Amount = amount;
        Unit = unit;
    }

    public BigDecimal Amount { get; }

    public string Unit { get; }

    public string Format(int fractionalDigits, IFormatProvider? provider = null) =>
        $"{Unit} {Amount.ToGroupedString(fractionalDigits, provider)}";

    [GeneratedRegex(@"^[\p{L}\p{N}][\p{L}\p{N} ._-]{0,23}$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidUnit();
}
