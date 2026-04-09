param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$Headed
)

$ErrorActionPreference = "Stop"

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

Invoke-Step -Action {
    dotnet build tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj -c $Configuration
} -ErrorMessage "La compilation du projet E2E a échoué."

Invoke-Step -Action {
    dotnet test tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj -c $Configuration --no-build --filter "Category=Validation"
} -ErrorMessage "L’exécution des tests E2E de validation a échoué."
