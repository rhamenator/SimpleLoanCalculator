using System.Globalization;
using SimpleLoanCalculator.Core.Loans;

namespace SimpleLoanCalculator_App;

public sealed class ScheduleRowView
{
    public string Period { get; set; } = string.Empty;
    public string Payment { get; set; } = string.Empty;
    public string Principal { get; set; } = string.Empty;
    public string Interest { get; set; } = string.Empty;
    public string Balance { get; set; } = string.Empty;

    public static ScheduleRowView From(AmortizationRow row, int scale) =>
        new(
        )
        {
            Period = row.Period.ToString(CultureInfo.CurrentCulture),
            Payment = row.Payment.Amount.ToGroupedString(scale),
            Principal = row.Principal.Amount.ToGroupedString(scale),
            Interest = row.Interest.Amount.ToGroupedString(scale),
            Balance = row.Balance.Amount.ToGroupedString(scale),
        };
}
