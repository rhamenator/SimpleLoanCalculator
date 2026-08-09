using System.Numerics;

namespace SimpleLoanCalculator.Core.Numerics;

/// <summary>
/// An exact rational number backed by <see cref="BigInteger"/>.
/// </summary>
public readonly record struct BigRational : IComparable<BigRational>
{
    public static readonly BigRational Zero = new(BigInteger.Zero, BigInteger.One);
    public static readonly BigRational One = new(BigInteger.One, BigInteger.One);

    public BigRational(BigInteger numerator)
        : this(numerator, BigInteger.One)
    {
    }

    public BigRational(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero)
        {
            throw new DivideByZeroException("A rational denominator cannot be zero.");
        }

        if (denominator.Sign < 0)
        {
            numerator = BigInteger.Negate(numerator);
            denominator = BigInteger.Negate(denominator);
        }

        if (numerator.IsZero)
        {
            Numerator = BigInteger.Zero;
            Denominator = BigInteger.One;
            return;
        }

        var divisor = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
        Numerator = numerator / divisor;
        Denominator = denominator / divisor;
    }

    public BigInteger Numerator { get; }

    public BigInteger Denominator { get; }

    public int Sign => Numerator.Sign;

    public BigRational Abs() => new(BigInteger.Abs(Numerator), Denominator);

    public BigRational Pow(int exponent)
    {
        if (exponent == 0)
        {
            return One;
        }

        if (exponent < 0)
        {
            if (Numerator.IsZero)
            {
                throw new DivideByZeroException("Zero cannot be raised to a negative power.");
            }

            return new BigRational(Denominator, Numerator).Pow(checked(-exponent));
        }

        var result = One;
        var factor = this;
        var remaining = exponent;

        while (remaining > 0)
        {
            if ((remaining & 1) != 0)
            {
                result *= factor;
            }

            remaining >>= 1;
            if (remaining > 0)
            {
                factor *= factor;
            }
        }

        return result;
    }

    public int CompareTo(BigRational other) =>
        (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);

    public static BigRational operator +(BigRational left, BigRational right) =>
        new(
            left.Numerator * right.Denominator + right.Numerator * left.Denominator,
            left.Denominator * right.Denominator);

    public static BigRational operator -(BigRational left, BigRational right) =>
        new(
            left.Numerator * right.Denominator - right.Numerator * left.Denominator,
            left.Denominator * right.Denominator);

    public static BigRational operator -(BigRational value) =>
        new(BigInteger.Negate(value.Numerator), value.Denominator);

    public static BigRational operator *(BigRational left, BigRational right) =>
        new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    public static BigRational operator /(BigRational left, BigRational right)
    {
        if (right.Numerator.IsZero)
        {
            throw new DivideByZeroException();
        }

        return new(
            left.Numerator * right.Denominator,
            left.Denominator * right.Numerator);
    }

    public static bool operator <(BigRational left, BigRational right) => left.CompareTo(right) < 0;

    public static bool operator >(BigRational left, BigRational right) => left.CompareTo(right) > 0;

    public static bool operator <=(BigRational left, BigRational right) => left.CompareTo(right) <= 0;

    public static bool operator >=(BigRational left, BigRational right) => left.CompareTo(right) >= 0;
}
