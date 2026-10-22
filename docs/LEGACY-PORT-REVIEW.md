# Legacy calculator review

## Source and scope

Maintenance objective: review the four recovered legacy commits on `master` and port useful behavior missing from `main`. The modern WinUI app and exact calculation engine remain the baseline. No additional operating-system UI is introduced.

| Legacy commit | Behavior or change | Disposition |
| --- | --- | --- |
| [b4efcf5](https://github.com/rhamenator/SimpleLoanCalculator/commit/b4efcf5) | Remove custom arrow/Enter focus navigation | Already satisfied: the WinUI form uses native keyboard behavior without those handlers. |
| [03a2a0e](https://github.com/rhamenator/SimpleLoanCalculator/commit/03a2a0e) | Rename present value to amount financed; clean up the worksheet | Modern labels already use amount financed. The legacy sale-price calculation workflow was missing and is restored through the existing principal solver plus an inverse purchase-worksheet action. |
| [81e60ab](https://github.com/rhamenator/SimpleLoanCalculator/commit/81e60ab) | C# financial routines with decimal overloads | Superseded by the independent arbitrary-precision engine; no legacy financial implementation is imported. |
| [410dba3](https://github.com/rhamenator/SimpleLoanCalculator/commit/410dba3) | Decimal UI/domain values and rate display rounding | Exact arithmetic already supersedes fixed-size decimal. Preserve current configurable precision rather than imposing the legacy four-place rate rounding. |

## Implementation and impact

- Add `LoanCalculator.CalculateSalePrice`: amount financed + trade-in − trade-in payoff + down payment, with matching-unit and positive-result validation.
- Add a localized worksheet action that fills sale price from the current financed amount and optional adjustments. Blank adjustments continue to mean zero.
- Correct reverse-input handling: principal, annual rate, or payment count is disabled and not parsed when it is the quantity being solved. Existing solvers supply that value before final loan validation and schedule calculation.
- Document the two-step target-payment-to-sale-price workflow in the README.
- Preserve existing loan math, display precision, native keyboard navigation, packaging, and Windows-only desktop scope.

The unknown-input fix is a derived requirement from the restored workflow: users must not need to invent a starting value for the quantity the app will calculate.

## Verification — October 22, 2026

- `dotnet test tests/SimpleLoanCalculator.Core.Tests/SimpleLoanCalculator.Core.Tests.csproj --no-restore`: 20 passed, 0 failed.
- `dotnet build src/SimpleLoanCalculator.App/SimpleLoanCalculator.App.csproj -p:Platform=x64 -p:RestoreLockedMode=true`: succeeded, 0 warnings, 0 errors.
- `dotnet run --project src/SimpleLoanCalculator.App/SimpleLoanCalculator.App.csproj -p:Platform=x64 --no-build`: launched a responsive Exact Loan Calculator window.
- Automated worksheet regressions cover positive and negative trade-in equity, amounts beyond decimal limits, mismatched units, and nonpositive principal/sale price.
- Desktop UI check: financed amount 250000 and blank adjustments populated sale price 250000 with success status.
- Desktop UI regression: clear amount financed, select Amount financed as the solve target, enter payment 1580.17 at 6.5 percent for 360 monthly payments, and calculate. The disabled blank field was accepted and filled with 249999.99; the cent difference reflects the rounded known payment.
- Code review confirmed the same unknown-input exclusion for annual rate and payment count. Those two UI paths were not separately exercised.
- `git diff --check`: passed.

Verification is maintainer-style self-review plus automated and desktop checks, not independent human review. UI interaction was checked manually through desktop automation because this repository currently has a core test project and no WinUI UI-test harness.
## Installation changes

Maintenance objective: make the Windows application easy to install. The existing x64/ARM64 self-contained installer workflow is retained. The setup now defaults to the current account under LocalAppData, does not request elevation, offers a desktop shortcut and launch-on-finish, and uses the approved icon in setup, uninstall, shortcuts, and the executable. Stable release asset names support permanent README download links. Tagged releases use their tag version and include SHA-256 checksums.

An existing all-users installation remains a separate installation; users should uninstall it before moving to the per-user version. The existing application ID remains unchanged for in-place per-user updates. Installers remain unsigned; the README states that limitation. No OS support or dependencies are added.

Verification: the local x64 app build passed with zero warnings/errors. The release workflow compiles both architecture installers and now installs, starts, checks for a responsive window, and uninstalls the x64 app on an isolated Windows runner. ARM64 installers are compiled but not executed on the x64 runner. Final CI results are recorded on the pull request.
