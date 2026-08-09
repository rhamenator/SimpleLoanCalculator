using System.Globalization;
using System.Numerics;

namespace SimpleLoanCalculator.Core.Numerics;

/// <summary>
/// An arbitrary-precision base-10 value represented by an integer coefficient
/// and a decimal scale. It deliberately excludes floating-point exponents.
/// </summary>
public readonly record struct BigDecimal : IComparable<BigDecimal>
{
    public static readonly BigDecimal Zero = new(BigInteger.Zero, 0);
    public static readonly BigDecimal One = new(BigInteger.One, 0);

    public BigDecimal(BigInteger unscaledValue, int scale)
    {
        if (scale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }

        while (scale > 0 && !unscaledValue.IsZero && unscaledValue % 10 == 0)
        {
            unscaledValue /= 10;
            scale--;
        }

        UnscaledValue = unscaledValue;
        Scale = unscaledValue.IsZero ? 0 : scale;
    }

    public BigInteger UnscaledValue { get; }

    public int Scale { get; }

    public int Sign => UnscaledValue.Sign;

    public BigRational ToRational() => new(UnscaledValue, Pow10(Scale));

    public BigDecimal Abs() => new(BigInteger.Abs(UnscaledValue), Scale);

    public BigDecimal Round(int scale, MidpointRounding mode = MidpointRounding.ToEven)
    {
        if (scale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }

        if (Scale <= scale)
        {
            return this;
        }

        var divisor = Pow10(Scale - scale);
        return new BigDecimal(DivideRounded(UnscaledValue, divisor, mode), scale);
    }

    public string ToPlainString(int? fractionalDigits = null)
    {
        var value = fractionalDigits.HasValue ? Round(fractionalDigits.Value) : this;
        var requestedScale = fractionalDigits ?? value.Scale;
        var negative = value.UnscaledValue.Sign < 0;
        var digits = BigInteger.Abs(value.UnscaledValue).ToString(CultureInfo.InvariantCulture);
        var naturalScale = value.Scale;

        if (naturalScale > 0)
        {
            digits = digits.PadLeft(naturalScale + 1, '0');
            digits = digits.Insert(digits.Length - naturalScale, ".");
        }

        if (requestedScale > naturalScale)
        {
            if (!digits.Contains('.', StringComparison.Ordinal))
            {
                digits += ".";
            }

            digits += new string('0', requestedScale - naturalScale);
        }

        return negative ? "-" + digits : digits;
    }

    public string ToGroupedString(int fractionalDigits, IFormatProvider? provider = null)
    {
        var nfi = NumberFormatInfo.GetInstance(provider ?? CultureInfo.CurrentCulture);
        var plain = Round(fractionalDigits).ToPlainString(fractionalDigits);
        var negative = plain.StartsWith("-", StringComparison.Ordinal);
        if (negative)
        {
            plain = plain[1..];
        }

        var parts = plain.Split('.');
        var integer = parts[0];
        var groups = new List<string>();
        for (var end = integer.Length; end > 0; end -= 3)
        {
            var start = Math.Max(0, end - 3);
            groups.Add(integer[start..end]);
        }

        groups.Reverse();
        var result = string.Join(nfi.NumberGroupSeparator, groups);
        if (fractionalDigits > 0)
        {
            result += nfi.NumberDecimalSeparator + (parts.Length > 1 ? parts[1] : new string('0', fractionalDigits));
        }

        return negative ? nfi.NegativeSign + result : result;
    }

    public override string ToString() => ToPlainString();

    public int CompareTo(BigDecimal other) => ToRational().CompareTo(other.ToRational());

    public static BigDecimal Parse(string text, IFormatProvider? provider = null)
    {
        if (!TryParse(text, provider, out var value))
        {
            throw new FormatException($"'{text}' is not a valid base-10 number.");
        }

        return value;
    }

    public static bool TryParse(string? text, IFormatProvider? provider, out BigDecimal value)
    {
        value = Zero;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var nfi = NumberFormatInfo.GetInstance(provider ?? CultureInfo.CurrentCulture);
        var candidate = text.Trim();
        var negative = false;

        if (candidate.StartsWith(nfi.NegativeSign, StringComparison.Ordinal))
        {
            negative = true;
            candidate = candidate[nfi.NegativeSign.Length..];
        }
        else if (candidate.StartsWith(nfi.PositiveSign, StringComparison.Ordinal))
        {
            candidate = candidate[nfi.PositiveSign.Length..];
        }

        foreach (var separator in new[] { nfi.NumberGroupSeparator, nfi.CurrencyGroupSeparator }.Distinct())
        {
            if (!string.IsNullOrEmpty(separator))
            {
                candidate = candidate.Replace(separator, string.Empty, StringComparison.Ordinal);
            }
        }

        var decimalSeparators = new[] { nfi.NumberDecimalSeparator, nfi.CurrencyDecimalSeparator }
            .Where(static separator => !string.IsNullOrEmpty(separator))
            .Distinct()
            .ToArray();
        string[] parts = [candidate];
        foreach (var separator in decimalSeparators)
        {
            if (candidate.Contains(separator, StringComparison.Ordinal))
            {
                parts = candidate.Split(separator, StringSplitOptions.None);
                break;
            }
        }

        if (parts.Length > 2 || parts.Length == 0 || parts[0].Length == 0)
        {
            return false;
        }

        var fraction = parts.Length == 2 ? parts[1] : string.Empty;
        if (parts.Length == 2 && fraction.Length == 0)
        {
            return false;
        }

        var digits = parts[0] + fraction;
        if (digits.Length == 0 || digits.Any(static c => c is < '0' or > '9'))
        {
            return false;
        }

        var unscaled = BigInteger.Parse(digits, NumberStyles.None, CultureInfo.InvariantCulture);
        value = new BigDecimal(negative ? BigInteger.Negate(unscaled) : unscaled, fraction.Length);
        return true;
    }

    public static BigDecimal FromRational(
        BigRational value,
        int scale,
        MidpointRounding mode = MidpointRounding.ToEven)
    {
        if (scale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale));
        }

        var scaledNumerator = value.Numerator * Pow10(scale);
        return new BigDecimal(DivideRounded(scaledNumerator, value.Denominator, mode), scale);
    }

    public static BigDecimal operator +(BigDecimal left, BigDecimal right)
    {
        var scale = Math.Max(left.Scale, right.Scale);
        var leftValue = left.UnscaledValue * Pow10(scale - left.Scale);
        var rightValue = right.UnscaledValue * Pow10(scale - right.Scale);
        return new BigDecimal(leftValue + rightValue, scale);
    }

    public static BigDecimal operator -(BigDecimal left, BigDecimal right) => left + -right;

    public static BigDecimal operator -(BigDecimal value) => new(BigInteger.Negate(value.UnscaledValue), value.Scale);

    public static BigDecimal operator *(BigDecimal left, BigDecimal right) =>
        new(left.UnscaledValue * right.UnscaledValue, checked(left.Scale + right.Scale));

    public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;

    public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;

    public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;

    public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;

    private static BigInteger Pow10(int exponent) => BigInteger.Pow(10, exponent);

    private static BigInteger DivideRounded(BigInteger numerator, BigInteger denominator, MidpointRounding mode)
    {
        if (denominator.Sign < 0)
        {
            numerator = BigInteger.Negate(numerator);
            denominator = BigInteger.Negate(denominator);
        }

        var quotient = BigInteger.DivRem(numerator, denominator, out var remainder);
        if (remainder.IsZero)
        {
            return quotient;
        }

        var sign = numerator.Sign * denominator.Sign;
        var comparison = (BigInteger.Abs(remainder) * 2).CompareTo(BigInteger.Abs(denominator));
        var increment = mode switch
        {
            MidpointRounding.ToZero => false,
            MidpointRounding.ToNegativeInfinity => sign < 0,
            MidpointRounding.ToPositiveInfinity => sign > 0,
            MidpointRounding.AwayFromZero => comparison >= 0,
            MidpointRounding.ToEven => comparison > 0 || (comparison == 0 && !quotient.IsEven),
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };

        return increment ? quotient + sign : quotient;
    }
}
