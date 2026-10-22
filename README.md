# Exact Loan Calculator

A modern Windows loan calculator for ordinary financing and extraordinary monetary scales. It calculates payments and full amortization schedules without `Microsoft.VisualBasic.Financial`, floating-point money, or fixed-size decimal limits.

The desktop UI uses C# and WinUI through the current Windows App SDK. The calculation engine is a separate .NET library so it can also be reused by projects such as LendWise.

## Install on Windows

Download the installer for your PC:

- **[Download for Windows x64](https://github.com/rhamenator/SimpleLoanCalculator/releases/latest/download/ExactLoanCalculator-win-x64-setup.exe)** — most Intel and AMD PCs.
- **[Download for Windows ARM64](https://github.com/rhamenator/SimpleLoanCalculator/releases/latest/download/ExactLoanCalculator-win-arm64-setup.exe)** — Windows on Arm PCs.

Run the downloaded setup file and choose **Install**. The app installs for your Windows account without requesting administrator access. Open **Exact Loan Calculator** from Start, or choose the optional desktop shortcut during setup. No .NET SDK, Developer Mode, or separate runtime installation is required.

Requires Windows 10 version 1809 or later. To check your PC type, open **Settings → System → About → System type**. Installers are currently unsigned, so Windows may display a publisher or reputation warning. Release downloads include SHA-256 checksums.

To update, run the newer installer for the same architecture. Remove the app through **Settings → Apps**. If an earlier version was installed for all users, uninstall that version first to avoid duplicate entries.

[All releases and checksums](https://github.com/rhamenator/SimpleLoanCalculator/releases/latest).

## What it does

- Calculates periodic payment, total paid, total interest, and a complete amortization schedule.
- Solves in reverse for principal, nominal annual rate, or number of payments.
- Supports end-of-period and beginning-of-period payments.
- Includes an optional purchase/trade-in worksheet.
- Accepts a user-defined monetary unit instead of assuming dollars.
- Uses arbitrary-size integers and exact rational intermediates, with configurable decimal display precision.
- Exports the displayed amortization schedule to the clipboard as CSV.
- Provides an English resource catalog as the starting point for additional UI localizations.

To work backward from a known payment, choose the quantity under **Solve for**, enter the other terms and the known payment, then calculate. The unknown field is disabled and does not require a starting value.

The purchase worksheet works in both directions: use the sale price and adjustments to fill the amount financed, or calculate a sale price from the amount financed. To find a purchase price for a target payment, first solve for **Amount financed**, then use **Calculate sale price from amount financed** in the worksheet.

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
