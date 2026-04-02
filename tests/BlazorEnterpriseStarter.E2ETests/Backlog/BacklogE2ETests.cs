using BlazorEnterpriseStarter.E2ETests.Infrastructure;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorEnterpriseStarter.E2ETests.Backlog;

[CollectionDefinition(nameof(E2ECollection))]
public sealed class E2ECollection : ICollectionFixture<E2ETestHostFixture>, ICollectionFixture<PlaywrightFixture>;

[Collection(nameof(E2ECollection))]
public sealed class BacklogE2ETests
{
    private readonly E2ETestHostFixture _hostFixture;
    private readonly PlaywrightFixture _playwrightFixture;

    public BacklogE2ETests(E2ETestHostFixture hostFixture, PlaywrightFixture playwrightFixture)
    {
        _hostFixture = hostFixture;
        _playwrightFixture = playwrightFixture;
    }

    [Fact]
    public async Task Backlog_devrait_afficher_la_page_et_les_indicateurs_de_synthese()
    {
        await using var browser = await _playwrightFixture.Playwright.Chromium.LaunchAsync(_playwrightFixture.CreateLaunchOptions());
        var page = await CreatePageAsync(browser);

        await page.GotoAsync(new Uri(_hostFixture.AppBaseAddress, "/backlog").ToString());

        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Backlog produit" })).ToBeVisibleAsync();
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Filtres et tri" })).ToBeVisibleAsync();
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Priorité critique" })).ToBeVisibleAsync();
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Vue active" })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Backlog_devrait_creer_un_element_et_le_retrouver_via_la_recherche()
    {
        await using var browser = await _playwrightFixture.Playwright.Chromium.LaunchAsync(_playwrightFixture.CreateLaunchOptions());
        var page = await CreatePageAsync(browser);
        var titre = $"Automatiser la revue E2E {DateTimeOffset.UtcNow:yyyyMMddHHmmss}";

        await page.GotoAsync(new Uri(_hostFixture.AppBaseAddress, "/backlog").ToString());

        var modal = await OuvrirModaleCreationAsync(page);

        await modal.GetByLabel("Titre").FillAsync(titre);
        await modal.GetByLabel("Description").FillAsync("Vérifier qu’un scénario utilisateur visible peut être validé de bout en bout.");
        var submitButton = modal.GetByRole(AriaRole.Button, new() { Name = "Créer l’élément" });
        await submitButton.ScrollIntoViewIfNeededAsync();
        await submitButton.ClickAsync(new() { Force = true });

        await Expect(page.GetByText("L’élément de backlog a été créé avec succès.")).ToBeVisibleAsync();

        await page.GetByLabel("Recherche").FillAsync(titre);

        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = titre })).ToBeVisibleAsync();
        await Expect(page.GetByText($"Recherche : {titre}")).ToBeVisibleAsync();
    }

    private static async Task<ILocator> OuvrirModaleCreationAsync(IPage page)
    {
        var openButton = page.GetByRole(AriaRole.Button, new() { Name = "Créer un élément" }).First;
        var modalHeading = page.GetByRole(AriaRole.Heading, new() { Name = "Créer un élément de backlog" });

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await openButton.ClickAsync();

            try
            {
                await Expect(modalHeading).ToBeVisibleAsync(new() { Timeout = 1_500 });
                return page.Locator(".app-modal");
            }
            catch (PlaywrightException)
            {
                await page.WaitForTimeoutAsync(400);
            }
        }

        throw new TimeoutException("La modale de création ne s’est pas ouverte dans le délai attendu.");
    }

    private static Task<IPage> CreatePageAsync(IBrowser browser) =>
        browser.NewPageAsync(new BrowserNewPageOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = 1600,
                Height = 1800
            }
        });
}
