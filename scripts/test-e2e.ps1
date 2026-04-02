param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$Headed
)

$ErrorActionPreference = "Stop"

if ($Headed) {
    $env:BES_E2E_HEADED = "true"
}

dotnet build tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj -c $Configuration
dotnet test tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj -c $Configuration --no-build
