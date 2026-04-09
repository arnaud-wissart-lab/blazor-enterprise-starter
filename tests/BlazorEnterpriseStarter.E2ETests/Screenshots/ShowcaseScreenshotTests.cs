using BlazorEnterpriseStarter.E2ETests.Infrastructure;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorEnterpriseStarter.E2ETests.Screenshots;

[Collection(nameof(E2ECollection))]
[Trait(E2ETestCategories.CategoryKey, E2ETestCategories.Screenshots)]
public sealed class ShowcaseScreenshotTests
{
    private static readonly ViewportSize ScreenshotViewport = new()
    {
        Width = 1600,
        Height = 1700
    };

    private readonly E2ETestHostFixture _hostFixture;
    private readonly PlaywrightFixture _playwrightFixture;
    private readonly string _screenshotsDirectory;

    public ShowcaseScreenshotTests(E2ETestHostFixture hostFixture, PlaywrightFixture playwrightFixture)
    {
        _hostFixture = hostFixture;
        _playwrightFixture = playwrightFixture;
        _screenshotsDirectory = RepositoryPaths.GetScreenshotsDirectory();
    }

    [Fact(DisplayName = "La capture de l’accueil doit être générée dans docs/screenshots")]
    public Task Generer_capture_accueil_async() =>
        GenerateScreenshotAsync(
            route: "/",
            fileName: "home-overview.png",
            readyState: static async page =>
            {
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Starter Blazor/.NET pour application métier" })).ToBeVisibleAsync();
                await Expect(page.GetByText("Point de contrôle simple entre l’interface et l’API.")).ToBeVisibleAsync();
                await page.Locator("text=Le contrôle est exécuté automatiquement au chargement.").WaitForAsync(new()
                {
                    State = WaitForSelectorState.Detached
                });
            });

    [Fact(DisplayName = "La capture de la bibliothèque UI doit être générée dans docs/screenshots")]
    public Task Generer_capture_bibliotheque_ui_async() =>
        GenerateScreenshotAsync(
            route: "/composants",
            fileName: "components-library.png",
            readyState: static async page =>
            {
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Bibliothèque UI" })).ToBeVisibleAsync();
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Les règles de base du système" })).ToBeVisibleAsync();
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Les composants utilisés au quotidien" })).ToBeVisibleAsync();
            });

    [Fact(DisplayName = "La capture du backlog doit être générée dans docs/screenshots")]
    public Task Generer_capture_backlog_async() =>
        GenerateScreenshotAsync(
            route: "/backlog",
            fileName: "backlog-module.png",
            readyState: static async page =>
            {
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Backlog produit" })).ToBeVisibleAsync();
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Filtres et tri" })).ToBeVisibleAsync();
                await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Vue active" })).ToBeVisibleAsync();
                await page.GetByText("Chargement du backlog produit").WaitForAsync(new()
                {
                    State = WaitForSelectorState.Detached
                });
            });

    private async Task GenerateScreenshotAsync(string route, string fileName, Func<IPage, Task> readyState)
    {
        Directory.CreateDirectory(_screenshotsDirectory);

        await using var browser = await _playwrightFixture.Playwright.Chromium.LaunchAsync(_playwrightFixture.CreateLaunchOptions());
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = ScreenshotViewport,
            ScreenSize = new ScreenSize
            {
                Width = ScreenshotViewport.Width,
                Height = ScreenshotViewport.Height
            },
            ColorScheme = ColorScheme.Light,
            Locale = "fr-FR",
            DeviceScaleFactor = 1
        });

        var page = await context.NewPageAsync();

        await page.AddInitScriptAsync("""
            () => {
                window.localStorage.setItem('bes-theme', 'light');
                document.documentElement.setAttribute('data-theme', 'light');
            }
            """);

        await page.GotoAsync(new Uri(_hostFixture.AppBaseAddress, route).ToString(), new()
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        await StabilizeForScreenshotAsync(page);
        await readyState(page);
        await page.WaitForTimeoutAsync(250);

        var screenshotPath = Path.Combine(_screenshotsDirectory, fileName);
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = screenshotPath,
            Type = ScreenshotType.Png,
            FullPage = false
        });
    }

    private static async Task StabilizeForScreenshotAsync(IPage page)
    {
        await page.AddStyleTagAsync(new()
        {
            Content = """
                *,
                *::before,
                *::after {
                    animation: none !important;
                    transition: none !important;
                    caret-color: transparent !important;
                }

                html {
                    scrollbar-width: none;
                }

                body::-webkit-scrollbar,
                *::-webkit-scrollbar {
                    display: none;
                }

                .theme-mode-toggle {
                    visibility: hidden !important;
                }
                """
        });

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
