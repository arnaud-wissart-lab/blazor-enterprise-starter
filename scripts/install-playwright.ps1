param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$project = "tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj"
$playwrightScript = "tests/BlazorEnterpriseStarter.E2ETests/bin/$Configuration/net10.0/playwright.ps1"

dotnet build $project -c $Configuration

if (-not (Test-Path $playwrightScript)) {
    throw "Le script Playwright n'a pas été généré : $playwrightScript"
}

pwsh $playwrightScript install chromium
