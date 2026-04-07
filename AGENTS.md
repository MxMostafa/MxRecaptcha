# Repository Guidelines

## Project Structure & Module Organization
This repository is a minimal .NET solution centered on `MxRecaptcha.slnx` and the library project in `MxRecaptcha/`. The project file is `MxRecaptcha/MxRecaptcha.csproj`, which currently targets `net10.0` with nullable reference types enabled. Place production code in `MxRecaptcha/*.cs` or feature folders under `MxRecaptcha/`. Build outputs belong in `MxRecaptcha/bin/` and `MxRecaptcha/obj/` and should not be edited manually. If tests are added, prefer a sibling test project such as `tests/MxRecaptcha.Tests/`.

## Build, Test, and Development Commands
- `dotnet restore MxRecaptcha.slnx` — restore NuGet packages for the solution.
- `dotnet build MxRecaptcha.slnx -c Debug` — compile the library locally.
- `dotnet build MxRecaptcha/MxRecaptcha.csproj -c Release` — verify the project in release mode.
- `dotnet test` — run tests once a test project exists.
- `dotnet format` — apply standard .NET formatting before opening a PR.

## Coding Style & Naming Conventions
Use standard C# conventions: 4-space indentation, file-scoped namespaces when practical, `PascalCase` for public types and members, `camelCase` for locals and parameters, and descriptive names over abbreviations. Keep one public type per file and match filenames to type names, for example `RecaptchaValidator.cs`. Respect nullable annotations and avoid suppressing warnings unless necessary.

## Testing Guidelines
There is no test project yet. When adding one, use xUnit or the existing team standard, place it under `tests/`, and name files after the unit under test, such as `RecaptchaValidatorTests.cs`. Keep tests deterministic and cover success, failure, and edge cases. Run `dotnet test` before submitting changes.

## Commit & Pull Request Guidelines
Git history is not available in this workspace snapshot, so use clear, imperative commit messages such as `Add token validation service` or `Fix nullable warnings in verifier`. Keep commits focused. Pull requests should include a short summary, testing notes, linked issues when applicable, and example request/response details for behavior changes.

## Security & Configuration Tips
Do not commit secrets, site keys, or private verification settings. Store environment-specific values outside source control and document required configuration in the PR when introducing new settings.
