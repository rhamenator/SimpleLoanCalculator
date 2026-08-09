# Exact Loan Calculator

A modern Windows loan calculator for ordinary financing and extraordinary monetary scales. It calculates payments and full amortization schedules without `Microsoft.VisualBasic.Financial`, floating-point money, or fixed-size decimal limits.

The desktop UI uses C# and WinUI through the current Windows App SDK. The calculation engine is a separate .NET library so it can also be reused by projects such as LendWise.

## What it does

- Calculates periodic payment, total paid, total interest, and a complete amortization schedule.
- Solves in reverse for principal, nominal annual rate, or number of payments.
- Supports end-of-period and beginning-of-period payments.
- Includes an optional purchase/trade-in worksheet.
- Accepts a user-defined monetary unit instead of assuming dollars.
- Uses arbitrary-size integers and exact rational intermediates, with configurable decimal display precision.
- Exports the displayed amortization schedule to the clipboard as CSV.
- Provides an English resource catalog as the starting point for additional UI localizations.

## Why the math stays practical

Normal payment calculations only require integer powers. `BigRational.Pow` computes those by exponentiation by squaring, rather than implementing a general-purpose arbitrary-precision logarithm or fractional-power library. Reverse rate solving uses a bounded bisection search over the monotonic payment function. This keeps the implementation understandable and responsive even for values far beyond `decimal`.

Interest is entered as a nominal annual percentage and divided by the number of payments per year. The engine supports nonnegative rates, 1–10,000 payments, 1–366 payments per year, and 0–18 displayed decimal places.

## Build and run

Requirements:

- Windows 10 version 1809 or later
- .NET 10 SDK
- Developer Mode enabled for command-line launch of the packaged app

```powershell
dotnet restore SimpleLoanCalculator.slnx --locked-mode
dotnet test tests/SimpleLoanCalculator.Core.Tests/SimpleLoanCalculator.Core.Tests.csproj
dotnet build src/SimpleLoanCalculator.App/SimpleLoanCalculator.App.csproj -p:Platform=x64
dotnet run --project src/SimpleLoanCalculator.App/SimpleLoanCalculator.App.csproj -p:Platform=x64
```

## Design

- `SimpleLoanCalculator.Core`: independent numeric, money, and loan-domain code.
- `SimpleLoanCalculator.App`: packaged WinUI desktop interface.
- `SimpleLoanCalculator.Core.Tests`: unit and performance-regression tests.

`BigDecimal` stores a `BigInteger` coefficient and decimal scale. `BigCurrency` pairs an amount with a validated unit label. `BigRational` keeps intermediate calculations exact and normalized. Currency conversion is deliberately out of scope: operands must use the same unit.

## Important limitation

This software is an educational estimation tool, not financial, tax, or legal advice. Real lenders may use different day-count conventions, compounding rules, fees, rounding rules, or final-payment adjustments. Verify any decision against the lender's disclosures.

See [SECURITY.md](SECURITY.md) for vulnerability reporting and [docs/PUBLICATION_READINESS.md](docs/PUBLICATION_READINESS.md) for the public-release audit.

## License

GPL-3.0-or-later. See [LICENSE](LICENSE).
