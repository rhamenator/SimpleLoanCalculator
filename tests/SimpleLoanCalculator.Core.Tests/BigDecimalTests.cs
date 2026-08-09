using System.Globalization;
using System.Numerics;
using SimpleLoanCalculator.Core.Numerics;

namespace SimpleLoanCalculator.Core.Tests;

public sealed class BigDecimalTests
{
    [Fact]
    public void Parse_preserves_an_amount_far_beyond_decimal_range()
    {
        var text = "999999999999999999999999999999999999999999999999999999.123456789012345678";

        var value = BigDecimal.Parse(text, CultureInfo.InvariantCulture);

        Assert.Equal(text, value.ToPlainString());
    }

    [Fact]
    public void Arithmetic_aligns_scales_without_floating_point()
    {
        var left = BigDecimal.Parse("1.25", CultureInfo.InvariantCulture);
        var right = BigDecimal.Parse("0.005", CultureInfo.InvariantCulture);

        Assert.Equal("1.255", (left + right).ToPlainString());
        Assert.Equal("0.00625", (left * right).ToPlainString());
    }

    [Fact]
    public void FromRational_rounds_half_to_even()
    {
        var half = new BigRational(new BigInteger(125), new BigInteger(100));

        Assert.Equal("1.2", BigDecimal.FromRational(half, 1).ToPlainString(1));
    }

    [Fact]
    public void Grouped_format_uses_requested_precision()
    {
        var value = BigDecimal.Parse("1234567890.5", CultureInfo.InvariantCulture);

        Assert.Equal("1,234,567,890.500", value.ToGroupedString(3, CultureInfo.InvariantCulture));
    }
}
