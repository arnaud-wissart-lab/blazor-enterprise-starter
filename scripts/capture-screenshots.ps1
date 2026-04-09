param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$Headed
)

$ErrorActionPreference = "Stop"

$project = "tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj"
$screenshotsDirectory = "docs/screenshots"

function Invoke-Step {
    param(
        [scriptblock]$Action,
        [string]$ErrorMessage
    )

    & $Action

    if ($LASTEXITCODE -ne 0) {
        throw $ErrorMessage
    }
}

if ($Headed) {
    $env:BES_E2E_HEADED = "true"
}

if (-not (Test-Path $screenshotsDirectory)) {
    New-Item -ItemType Directory -Path $screenshotsDirectory | Out-Null
}

Write-Host "Compilation du projet E2E..." -ForegroundColor Cyan
Invoke-Step -Action {
    dotnet build $project -c $Configuration
} -ErrorMessage "La compilation du projet E2E a échoué."

Write-Host "Génération des captures Playwright..." -ForegroundColor Cyan
Invoke-Step -Action {
    dotnet test $project -c $Configuration --no-build --filter "Category=Screenshots"
} -ErrorMessage "La génération des captures Playwright a échoué."

Write-Host ""
Write-Host "Captures générées :" -ForegroundColor Green
Write-Host " - docs/screenshots/home-overview.png"
Write-Host " - docs/screenshots/components-library.png"
Write-Host " - docs/screenshots/backlog-module.png"
Write-Host ""
Write-Host "Capture AppHost/Aspire : manuelle pour le moment." -ForegroundColor Yellow
