using SimpleLoanCalculator.Core.Money;
using SimpleLoanCalculator.Core.Numerics;

namespace SimpleLoanCalculator.Core.Loans;

public enum PaymentTiming
{
    EndOfPeriod,
    BeginningOfPeriod,
}

public enum LoanSolveTarget
{
    PeriodicPayment,
    Principal,
    AnnualRate,
    NumberOfPayments,
}

public sealed record LoanTerms(
    BigCurrency Principal,
    BigDecimal AnnualRatePercent,
    int NumberOfPayments,
    int PaymentsPerYear,
    PaymentTiming PaymentTiming,
    int DisplayScale)
{
    public void Validate()
    {
        if (Principal.Amount <= BigDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(Principal), "Principal must be greater than zero.");
        }

        if (AnnualRatePercent < BigDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(AnnualRatePercent), "Annual rate cannot be negative.");
        }

        if (NumberOfPayments is < 1 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(NumberOfPayments), "Payments must be between 1 and 10,000.");
        }

        if (PaymentsPerYear is < 1 or > 366)
        {
            throw new ArgumentOutOfRangeException(nameof(PaymentsPerYear), "Payments per year must be between 1 and 366.");
        }

        if (DisplayScale is < 0 or > 18)
        {
            throw new ArgumentOutOfRangeException(nameof(DisplayScale), "Display precision must be between 0 and 18.");
        }
    }
}

public sealed record AmortizationRow(
    int Period,
    BigCurrency Payment,
    BigCurrency Principal,
    BigCurrency Interest,
    BigCurrency Balance);

public sealed record LoanSummary(
    LoanTerms Terms,
    BigCurrency PeriodicPayment,
    BigCurrency TotalPaid,
    BigCurrency TotalInterest,
    IReadOnlyList<AmortizationRow> Schedule);
