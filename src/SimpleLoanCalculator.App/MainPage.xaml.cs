using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleLoanCalculator.Core.Loans;
using SimpleLoanCalculator.Core.Money;
using SimpleLoanCalculator.Core.Numerics;
using Windows.ApplicationModel.DataTransfer;

namespace SimpleLoanCalculator_App;

public sealed partial class MainPage : Page
{
    private LoanSummary? _summary;

    public MainPage()
    {
        InitializeComponent();
    }

    public ObservableCollection<ScheduleRowView> ScheduleRows { get; } = [];

    private async void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            CalculateButton.IsEnabled = false;
            StatusInfoBar.IsOpen = false;
            var input = ReadInput();
            var result = await Task.Run(() => Calculate(input));
            ApplyResult(result);
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or InvalidOperationException or OverflowException)
        {
            ShowStatus(InfoBarSeverity.Error, "Check the loan inputs", exception.Message);
        }
        finally
        {
            CalculateButton.IsEnabled = true;
        }
    }

    private void SolveTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (KnownPaymentTextBox is null || SolveTargetComboBox.SelectedItem is not ComboBoxItem item)
        {
            return;
        }

        KnownPaymentTextBox.IsEnabled = !string.Equals(
            item.Tag?.ToString(),
            nameof(LoanSolveTarget.PeriodicPayment),
            StringComparison.Ordinal);
    }

    private void UseAmountFinanced_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var unit = MonetaryUnitTextBox.Text;
            var amount = LoanCalculator.CalculateAmountFinanced(
                Money(SalePriceTextBox.Text, unit, "Sale price"),
                MoneyOrZero(TradeInTextBox.Text, unit, "Trade-in value"),
                MoneyOrZero(TradeInPayoffTextBox.Text, unit, "Trade-in payoff"),
                MoneyOrZero(DownPaymentTextBox.Text, unit, "Down payment"));

            PrincipalTextBox.Text = amount.Amount.ToPlainString();
            ShowStatus(InfoBarSeverity.Success, "Amount financed updated", amount.Format(ReadDisplayScale()));
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or InvalidOperationException)
        {
            ShowStatus(InfoBarSeverity.Error, "Check the purchase worksheet", exception.Message);
        }
    }

    private void CopySchedule_Click(object sender, RoutedEventArgs e)
    {
        if (_summary is null)
        {
            return;
        }

        var csv = new StringBuilder();
        csv.AppendLine("period,unit,payment,principal,interest,balance");
        foreach (var row in _summary.Schedule)
        {
            csv.Append(row.Period).Append(',')
                .Append(EscapeCsv(row.Payment.Unit)).Append(',')
                .Append(row.Payment.Amount.ToPlainString(_summary.Terms.DisplayScale)).Append(',')
                .Append(row.Principal.Amount.ToPlainString(_summary.Terms.DisplayScale)).Append(',')
                .Append(row.Interest.Amount.ToPlainString(_summary.Terms.DisplayScale)).Append(',')
                .Append(row.Balance.Amount.ToPlainString(_summary.Terms.DisplayScale)).AppendLine();
        }

        var package = new DataPackage();
        package.SetText(csv.ToString());
        Clipboard.SetContent(package);
        ShowStatus(InfoBarSeverity.Success, "Schedule copied", "CSV data is ready to paste into a text file or spreadsheet.");
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        PrincipalTextBox.Text = "250000";
        AnnualRateTextBox.Text = "6.5";
        NumberOfPaymentsTextBox.Text = "360";
        PaymentsPerYearTextBox.Text = "12";
        MonetaryUnitTextBox.Text = "USD";
        DisplayScaleTextBox.Text = "2";
        KnownPaymentTextBox.Text = string.Empty;
        SalePriceTextBox.Text = string.Empty;
        TradeInTextBox.Text = string.Empty;
        TradeInPayoffTextBox.Text = string.Empty;
        DownPaymentTextBox.Text = string.Empty;
        PaymentTimingComboBox.SelectedIndex = 0;
        SolveTargetComboBox.SelectedIndex = 0;
        _summary = null;
        ScheduleRows.Clear();
        PaymentResultTextBlock.Text = "—";
        TotalPaidResultTextBlock.Text = "—";
        TotalInterestResultTextBlock.Text = "—";
        ResultDescriptionTextBlock.Text = "Calculate a loan to see its payment and amortization schedule.";
        CopyScheduleButton.IsEnabled = false;
        StatusInfoBar.IsOpen = false;
    }

    private CalculationInput ReadInput()
    {
        var culture = CultureInfo.CurrentCulture;
        var unit = MonetaryUnitTextBox.Text;
        var principal = Money(PrincipalTextBox.Text, unit, "Amount financed");
        var annualRate = ParseDecimal(AnnualRateTextBox.Text, "Annual rate", culture);
        var payments = ParseInteger(NumberOfPaymentsTextBox.Text, "Number of payments", 1, 10_000);
        var paymentsPerYear = ParseInteger(PaymentsPerYearTextBox.Text, "Payments per year", 1, 366);
        var scale = ReadDisplayScale();
        var timing = ((PaymentTimingComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString()) switch
        {
            nameof(PaymentTiming.BeginningOfPeriod) => PaymentTiming.BeginningOfPeriod,
            _ => PaymentTiming.EndOfPeriod,
        };
        var target = Enum.Parse<LoanSolveTarget>(
            (SolveTargetComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString()
                ?? nameof(LoanSolveTarget.PeriodicPayment));
        BigCurrency? knownPayment = target == LoanSolveTarget.PeriodicPayment
            ? null
            : Money(KnownPaymentTextBox.Text, unit, "Known periodic payment");

        return new CalculationInput(principal, annualRate, payments, paymentsPerYear, timing, scale, target, knownPayment);
    }

    private static CalculationResult Calculate(CalculationInput input)
    {
        var principal = input.Principal;
        var rate = input.AnnualRatePercent;
        var payments = input.NumberOfPayments;
        string? solvedMessage = null;
        var knownPayment = input.KnownPayment;

        switch (input.Target)
        {
            case LoanSolveTarget.Principal:
                principal = LoanCalculator.SolvePrincipal(
                    knownPayment ?? throw new InvalidOperationException("A known payment is required to solve principal."),
                    rate, payments, input.PaymentsPerYear, input.Timing, input.Scale);
                solvedMessage = $"Solved amount financed: {principal.Format(input.Scale)}";
                break;
            case LoanSolveTarget.AnnualRate:
                rate = LoanCalculator.SolveAnnualRatePercent(
                    principal,
                    knownPayment ?? throw new InvalidOperationException("A known payment is required to solve rate."),
                    payments, input.PaymentsPerYear, input.Timing);
                solvedMessage = $"Solved nominal annual rate: {rate.ToPlainString(8)}%";
                break;
            case LoanSolveTarget.NumberOfPayments:
                payments = LoanCalculator.SolveNumberOfPayments(
                    principal,
                    knownPayment ?? throw new InvalidOperationException("A known payment is required to solve the number of payments."),
                    rate, input.PaymentsPerYear, input.Timing);
                solvedMessage = $"Solved number of payments: {payments}";
                break;
        }

        var terms = new LoanTerms(principal, rate, payments, input.PaymentsPerYear, input.Timing, input.Scale);
        return new CalculationResult(LoanCalculator.Calculate(terms), solvedMessage, rate, payments, principal);
    }

    private void ApplyResult(CalculationResult result)
    {
        _summary = result.Summary;
        PrincipalTextBox.Text = result.Principal.Amount.ToPlainString();
        AnnualRateTextBox.Text = result.AnnualRate.ToPlainString();
        NumberOfPaymentsTextBox.Text = result.NumberOfPayments.ToString(CultureInfo.CurrentCulture);
        PaymentResultTextBlock.Text = result.Summary.PeriodicPayment.Format(result.Summary.Terms.DisplayScale);
        TotalPaidResultTextBlock.Text = result.Summary.TotalPaid.Format(result.Summary.Terms.DisplayScale);
        TotalInterestResultTextBlock.Text = result.Summary.TotalInterest.Format(result.Summary.Terms.DisplayScale);
        ResultDescriptionTextBlock.Text = result.SolvedMessage ??
            $"{result.Summary.Terms.NumberOfPayments:N0} payments at {result.Summary.Terms.AnnualRatePercent.ToPlainString()}% nominal annual rate.";

        ScheduleRows.Clear();
        foreach (var row in result.Summary.Schedule)
        {
            ScheduleRows.Add(ScheduleRowView.From(row, result.Summary.Terms.DisplayScale));
        }

        CopyScheduleButton.IsEnabled = true;
        ShowStatus(
            InfoBarSeverity.Success,
            "Calculation complete",
            result.SolvedMessage ?? "The amortization schedule uses exact internal arithmetic and explicit display rounding.");
    }

    private int ReadDisplayScale() =>
        ParseInteger(DisplayScaleTextBox.Text, "Decimal places", 0, 18);

    private static BigCurrency Money(string text, string unit, string fieldName) =>
        new(ParseDecimal(text, fieldName, CultureInfo.CurrentCulture), unit);

    private static BigCurrency MoneyOrZero(string text, string unit, string fieldName) =>
        string.IsNullOrWhiteSpace(text)
            ? new BigCurrency(BigDecimal.Zero, unit)
            : Money(text, unit, fieldName);

    private static BigDecimal ParseDecimal(string text, string fieldName, IFormatProvider provider)
    {
        if (!BigDecimal.TryParse(text, provider, out var value))
        {
            throw new FormatException($"{fieldName} must be a base-10 number without exponent notation.");
        }

        return value;
    }

    private static int ParseInteger(string text, string fieldName, int minimum, int maximum)
    {
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var value) ||
            value < minimum || value > maximum)
        {
            throw new FormatException($"{fieldName} must be a whole number from {minimum:N0} through {maximum:N0}.");
        }

        return value;
    }

    private void ShowStatus(InfoBarSeverity severity, string title, string message)
    {
        StatusInfoBar.Severity = severity;
        StatusInfoBar.Title = title;
        StatusInfoBar.Message = message;
        StatusInfoBar.IsOpen = true;
    }

    private static string EscapeCsv(string value) =>
        '"' + value.Replace("\"", "\"\"", StringComparison.Ordinal) + '"';

    private sealed record CalculationInput(
        BigCurrency Principal,
        BigDecimal AnnualRatePercent,
        int NumberOfPayments,
        int PaymentsPerYear,
        PaymentTiming Timing,
        int Scale,
        LoanSolveTarget Target,
        BigCurrency? KnownPayment);

    private sealed record CalculationResult(
        LoanSummary Summary,
        string? SolvedMessage,
        BigDecimal AnnualRate,
        int NumberOfPayments,
        BigCurrency Principal);
}
