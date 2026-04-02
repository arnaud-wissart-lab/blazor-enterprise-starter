using Microsoft.Playwright;

namespace BlazorEnterpriseStarter.E2ETests.Infrastructure;

/// <summary>
/// Centralise la création Playwright pour les scénarios E2E.
/// </summary>
public sealed class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = default!;

    public Task InitializeAsync() =>
        InitializeCoreAsync();

    public async Task DisposeAsync()
    {
        Playwright?.Dispose();
        await Task.CompletedTask;
    }

    public BrowserTypeLaunchOptions CreateLaunchOptions() =>
        new()
        {
            Headless = !string.Equals(Environment.GetEnvironmentVariable("BES_E2E_HEADED"), "true", StringComparison.OrdinalIgnoreCase)
        };

    private async Task InitializeCoreAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    }
}
