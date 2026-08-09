using System.Diagnostics;
using System.Globalization;
using SimpleLoanCalculator.Core.Loans;
using SimpleLoanCalculator.Core.Money;
using SimpleLoanCalculator.Core.Numerics;

namespace SimpleLoanCalculator.Core.Tests;

public sealed class LoanCalculatorTests
{
    [Fact]
    public void Standard_thirty_year_loan_matches_known_payment()
    {
        var summary = LoanCalculator.Calculate(Terms("250000", "6.5", 360));

        Assert.Equal("1580.17", summary.PeriodicPayment.Amount.ToPlainString(2));
        Assert.Equal("0.00", summary.Schedule[^1].Balance.Amount.ToPlainString(2));
        Assert.Equal(360, summary.Schedule.Count);
    }

    [Fact]
    public void Beginning_of_period_payment_is_lower_than_end_of_period_payment()
    {
        var end = LoanCalculator.Calculate(Terms("100000", "7", 120, PaymentTiming.EndOfPeriod));
        var beginning = LoanCalculator.Calculate(Terms("100000", "7", 120, PaymentTiming.BeginningOfPeriod));

        Assert.True(beginning.PeriodicPayment.Amount < end.PeriodicPayment.Amount);
        Assert.Equal("0.00", beginning.Schedule[^1].Balance.Amount.ToPlainString(2));
    }

    [Fact]
    public void Principal_solver_round_trips_the_payment()
    {
        var original = LoanCalculator.Calculate(Terms("987654321.1234", "12.75", 84));

        var solved = LoanCalculator.SolvePrincipal(
            original.PeriodicPayment,
            BigDecimal.Parse("12.75", CultureInfo.InvariantCulture),
            84,
            12,
            PaymentTiming.EndOfPeriod,
            4);

        var difference = (solved.Amount - termsPrincipal()).Abs();
        Assert.True(difference < BigDecimal.Parse("0.2", CultureInfo.InvariantCulture));

        static BigDecimal termsPrincipal() =>
            BigDecimal.Parse("987654321.1234", CultureInfo.InvariantCulture);
    }

    [Fact]
    public void Rate_solver_recovers_a_known_rate_without_logarithms()
    {
        var terms = Terms("250000", "6.5", 360);
        var summary = LoanCalculator.Calculate(terms);

        var solved = LoanCalculator.SolveAnnualRatePercent(
            terms.Principal,
            summary.PeriodicPayment,
            terms.NumberOfPayments,
            terms.PaymentsPerYear,
            terms.PaymentTiming,
            8);

        var difference = (solved - BigDecimal.Parse("6.5", CultureInfo.InvariantCulture)).Abs();
        Assert.True(difference < BigDecimal.Parse("0.000001", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Period_solver_recovers_a_known_term()
    {
        var terms = Terms("250000", "6.5", 360);
        var summary = LoanCalculator.Calculate(terms);

        var solved = LoanCalculator.SolveNumberOfPayments(
            terms.Principal,
            summary.PeriodicPayment,
            terms.AnnualRatePercent,
            terms.PaymentsPerYear,
            terms.PaymentTiming);

        Assert.Equal(360, solved);
    }

    [Fact]
    public void Purchase_worksheet_preserves_the_legacy_financing_rule()
    {
        var amount = LoanCalculator.CalculateAmountFinanced(
            Money("50000"),
            Money("12000"),
            Money("4000"),
            Money("5000"));

        Assert.Equal("37000", amount.Amount.ToPlainString());
    }

    [Fact]
    public void Hyperinflation_scale_input_does_not_overflow_or_require_custom_transcendentals()
    {
        var hugePrincipal = "1" + new string('0', 120);
        var watch = Stopwatch.StartNew();

        var summary = LoanCalculator.Calculate(Terms(hugePrincipal, "1000000", 24, scale: 8));

        watch.Stop();
        Assert.True(summary.PeriodicPayment.Amount > BigDecimal.Zero);
        Assert.StartsWith("USD ", summary.PeriodicPayment.Format(8, CultureInfo.InvariantCulture));
        Assert.True(watch.Elapsed < TimeSpan.FromSeconds(5), $"Calculation took {watch.Elapsed}.");
    }

    [Fact]
    public void Different_units_are_rejected_instead_of_silently_converted()
    {
        var usd = Money("100", "USD");
        var eur = Money("10", "EUR");

        Assert.Throws<InvalidOperationException>(() =>
            LoanCalculator.CalculateAmountFinanced(usd, eur, usd, usd));
    }

    private static LoanTerms Terms(
        string principal,
        string rate,
        int payments,
        PaymentTiming timing = PaymentTiming.EndOfPeriod,
        int scale = 2) =>
        new(
            Money(principal),
            BigDecimal.Parse(rate, CultureInfo.InvariantCulture),
            payments,
            12,
            timing,
            scale);

    private static BigCurrency Money(string value, string unit = "USD") =>
        new(BigDecimal.Parse(value, CultureInfo.InvariantCulture), unit);
}
