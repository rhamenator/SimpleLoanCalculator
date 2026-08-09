# Publication readiness review

Reviewed: 2026-08-09

## Scope

The previous learning-project implementation and Git history were intentionally retired. A complete local backup was made before the rebuild. This review covers the replacement C#/WinUI application, reusable calculation library, tests, documentation, and automation.

The local replacement repository starts on `main`; the untouched private GitHub repository still uses the legacy `master` branch. Publishing therefore requires an intentional remote-history replacement and default-branch change.

## Checks completed

- Full historical secret scan of the legacy repository with Gitleaks 8.30.1: no findings in 13 commits.
- Gitleaks scan of the complete replacement working tree: no findings.
- Replacement solution builds with zero warnings.
- All 12 calculation tests pass, including known loan examples, reverse solves, monetary-unit validation, beginning-of-period payments, purchase financing, and a 121-digit high-rate performance case.
- The UI launches with package identity and creates a responsive `Exact Loan Calculator` window.
- No network, authentication, database, lender, or brokerage integration is present.
- GitHub Actions dependencies are pinned to full commit SHAs and Dependabot is configured.
- NuGet's direct and transitive vulnerability check reports no known vulnerable packages.

## Design decisions

- Monetary amounts never use binary floating point.
- Exact rational intermediates avoid cumulative rounding drift.
- Integer powers use exponentiation by squaring; reverse rate solving uses bounded bisection instead of custom transcendental functions.
- Units are labels, not exchange rates. Mixed-unit arithmetic is rejected.
- Inputs have explicit limits to bound memory and runtime.

## Residual limitations

- The model uses a nominal annual percentage divided evenly across payment periods.
- It does not model fees, irregular dates, day-count conventions, variable rates, taxes, insurance, or currency conversion.
- Displayed schedule rows are rounded for presentation; exact values remain internal.
- Financial institutions may apply different rounding and final-payment rules.
- A production signing identity and distributable MSIX are not configured; the
  checked-in manifest retains a local development publisher identity.

These limits are documented in the application and README. The replacement is licensed GPL-3.0-or-later and is suitable for public review after the owner replaces the remote history when publishing.
