using System.Numerics;
using SimpleLoanCalculator.Core.Money;
using SimpleLoanCalculator.Core.Numerics;

namespace SimpleLoanCalculator.Core.Loans;

public static class LoanCalculator
{
    private static readonly BigRational OneHundred = new(100);

    public static LoanSummary Calculate(LoanTerms terms)
    {
        terms.Validate();
        var principal = terms.Principal.Amount.ToRational();
        var rate = PeriodicRate(terms.AnnualRatePercent, terms.PaymentsPerYear);
        var payment = PaymentFor(principal, rate, terms.NumberOfPayments, terms.PaymentTiming);
        var rows = BuildSchedule(terms, principal, rate, payment);
        var totalPaid = payment * new BigRational(terms.NumberOfPayments);
        var totalInterest = totalPaid - principal;

        return new LoanSummary(
            terms,
            Currency(payment, terms),
            Currency(totalPaid, terms),
            Currency(totalInterest, terms),
            rows);
    }

    public static BigCurrency SolvePrincipal(
        BigCurrency periodicPayment,
        BigDecimal annualRatePercent,
        int numberOfPayments,
        int paymentsPerYear,
        PaymentTiming timing,
        int outputScale)
    {
        ValidatePayment(periodicPayment);
        var rate = PeriodicRate(annualRatePercent, paymentsPerYear);
        var paymentPerUnit = PaymentFor(BigRational.One, rate, numberOfPayments, timing);
        var principal = periodicPayment.Amount.ToRational() / paymentPerUnit;
        return new BigCurrency(BigDecimal.FromRational(principal, outputScale), periodicPayment.Unit);
    }

    public static int SolveNumberOfPayments(
        BigCurrency principal,
        BigCurrency periodicPayment,
        BigDecimal annualRatePercent,
        int paymentsPerYear,
        PaymentTiming timing)
    {
        ValidatePayment(principal);
        ValidatePayment(periodicPayment);
        EnsureSameUnit(principal, periodicPayment);
        var principalValue = principal.Amount.ToRational();
        var targetPayment = periodicPayment.Amount.ToRational();
        var paymentScale = periodicPayment.Amount.Scale;
        var rate = PeriodicRate(annualRatePercent, paymentsPerYear);

        if (rate.Sign == 0)
        {
            return Ceiling(principalValue / targetPayment);
        }

        var interestOnly = timing == PaymentTiming.EndOfPeriod
            ? principalValue * rate
            : principalValue * rate / (BigRational.One + rate);
        if (targetPayment <= interestOnly)
        {
            throw new InvalidOperationException("The payment does not exceed periodic interest, so the balance never amortizes.");
        }

        var low = 1;
        var high = 2;
        bool RequiresMorePayment(int periods) =>
            BigDecimal.FromRational(PaymentFor(principalValue, rate, periods, timing), paymentScale)
                .ToRational() > targetPayment;

        while (high < 10_000 && RequiresMorePayment(high))
        {
            low = high;
            high = Math.Min(10_000, high * 2);
        }

        if (RequiresMorePayment(high))
        {
            throw new InvalidOperationException("The payment requires more than 10,000 periods.");
        }

        while (low + 1 < high)
        {
            var middle = low + ((high - low) / 2);
            if (!RequiresMorePayment(middle))
            {
                high = middle;
            }
            else
            {
                low = middle;
            }
        }

        return high;
    }

    public static BigDecimal SolveAnnualRatePercent(
        BigCurrency principal,
        BigCurrency periodicPayment,
        int numberOfPayments,
        int paymentsPerYear,
        PaymentTiming timing,
        int resultScale = 12)
    {
        ValidatePayment(principal);
        ValidatePayment(periodicPayment);
        EnsureSameUnit(principal, periodicPayment);
        if (numberOfPayments is < 1 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfPayments));
        }

        var principalValue = principal.Amount.ToRational();
        var targetPayment = periodicPayment.Amount.ToRational();
        var zeroRatePayment = principalValue / new BigRational(numberOfPayments);
        if (targetPayment < zeroRatePayment)
        {
            throw new InvalidOperationException("The payment is below the zero-interest payment; negative rates are not solved.");
        }

        if (targetPayment == zeroRatePayment)
        {
            return BigDecimal.Zero;
        }

        var lowPercent = BigRational.Zero;
        var highPercent = new BigRational(100);
        var maxPercent = new BigRational(BigInteger.Pow(10, 12));

        while (PaymentAtAnnualPercent(principalValue, highPercent, numberOfPayments, paymentsPerYear, timing) < targetPayment)
        {
            highPercent *= new BigRational(2);
            if (highPercent > maxPercent)
            {
                throw new InvalidOperationException("No nonnegative annual rate up to 1,000,000,000,000% matches the payment.");
            }
        }

        var iterations = Math.Max(64, resultScale * 5);
        for (var index = 0; index < iterations; index++)
        {
            var middle = (lowPercent + highPercent) / new BigRational(2);
            var candidate = PaymentAtAnnualPercent(principalValue, middle, numberOfPayments, paymentsPerYear, timing);
            if (candidate < targetPayment)
            {
                lowPercent = middle;
            }
            else
            {
                highPercent = middle;
            }
        }

        return BigDecimal.FromRational((lowPercent + highPercent) / new BigRational(2), resultScale);
    }

    public static BigCurrency CalculateAmountFinanced(
        BigCurrency salePrice,
        BigCurrency tradeIn,
        BigCurrency tradeInPayoff,
        BigCurrency downPayment)
    {
        EnsureSameUnit(salePrice, tradeIn);
        EnsureSameUnit(salePrice, tradeInPayoff);
        EnsureSameUnit(salePrice, downPayment);
        var amount = salePrice.Amount - tradeIn.Amount + tradeInPayoff.Amount - downPayment.Amount;
        if (amount <= BigDecimal.Zero)
        {
            throw new InvalidOperationException("The financing adjustments produce a nonpositive amount financed.");
        }

        return new BigCurrency(amount, salePrice.Unit);
    }

    private static IReadOnlyList<AmortizationRow> BuildSchedule(
        LoanTerms terms,
        BigRational startingPrincipal,
        BigRational rate,
        BigRational payment)
    {
        var rows = new List<AmortizationRow>(terms.NumberOfPayments);
        var balance = startingPrincipal;
        var carriedInterest = BigRational.Zero;

        for (var period = 1; period <= terms.NumberOfPayments; period++)
        {
            BigRational interest;
            BigRational principalPaid;

            if (terms.PaymentTiming == PaymentTiming.EndOfPeriod)
            {
                interest = balance * rate;
                principalPaid = payment - interest;
                balance = balance + interest - payment;
            }
            else
            {
                interest = carriedInterest;
                principalPaid = payment - interest;
                balance -= payment;
                if (period < terms.NumberOfPayments)
                {
                    carriedInterest = balance * rate;
                    balance += carriedInterest;
                }
                else
                {
                    carriedInterest = BigRational.Zero;
                }
            }

            if (period == terms.NumberOfPayments ||
                balance.Abs() < new BigRational(BigInteger.One, BigInteger.Pow(10, terms.DisplayScale + 4)))
            {
                balance = BigRational.Zero;
            }

            rows.Add(new AmortizationRow(
                period,
                Currency(payment, terms),
                Currency(principalPaid, terms),
                Currency(interest, terms),
                Currency(balance, terms)));
        }

        return rows;
    }

    private static BigRational PaymentAtAnnualPercent(
        BigRational principal,
        BigRational annualPercent,
        int payments,
        int paymentsPerYear,
        PaymentTiming timing) =>
        PaymentFor(principal, annualPercent / OneHundred / new BigRational(paymentsPerYear), payments, timing);

    private static BigRational PaymentFor(
        BigRational principal,
        BigRational periodicRate,
        int payments,
        PaymentTiming timing)
    {
        if (payments is < 1 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(payments));
        }

        if (periodicRate.Sign == 0)
        {
            return principal / new BigRational(payments);
        }

        var growth = (BigRational.One + periodicRate).Pow(payments);
        var payment = principal * periodicRate * growth / (growth - BigRational.One);
        return timing == PaymentTiming.BeginningOfPeriod
            ? payment / (BigRational.One + periodicRate)
            : payment;
    }

    private static BigRational PeriodicRate(BigDecimal annualRatePercent, int paymentsPerYear)
    {
        if (annualRatePercent < BigDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(annualRatePercent));
        }

        if (paymentsPerYear is < 1 or > 366)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentsPerYear));
        }

        return annualRatePercent.ToRational() / OneHundred / new BigRational(paymentsPerYear);
    }

    private static BigCurrency Currency(BigRational value, LoanTerms terms) =>
        new(BigDecimal.FromRational(value, terms.DisplayScale), terms.Principal.Unit);

    private static int Ceiling(BigRational value)
    {
        var quotient = BigInteger.DivRem(value.Numerator, value.Denominator, out var remainder);
        if (!remainder.IsZero && value.Sign > 0)
        {
            quotient++;
        }

        if (quotient > int.MaxValue)
        {
            throw new OverflowException("The number of payments exceeds the supported range.");
        }

        return (int)quotient;
    }

    private static void ValidatePayment(BigCurrency amount)
    {
        if (amount.Amount <= BigDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }
    }

    private static void EnsureSameUnit(BigCurrency left, BigCurrency right)
    {
        if (!string.Equals(left.Unit, right.Unit, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Monetary units must match; this calculator does not perform currency conversion.");
        }
    }
}
