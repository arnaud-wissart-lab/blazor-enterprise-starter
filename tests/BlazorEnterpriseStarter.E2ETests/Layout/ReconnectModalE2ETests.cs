using BlazorEnterpriseStarter.E2ETests.Infrastructure;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorEnterpriseStarter.E2ETests.Layout;

[Collection(nameof(E2ECollection))]
[Trait(E2ETestCategories.CategoryKey, E2ETestCategories.Validation)]
public sealed class ReconnectModalE2ETests
{
    private static readonly string[] TextesParasitesAttendus =
    [
        "Connexion interrompue",
        "Reconnexion au serveur...",
        "Impossible de rétablir la session",
        "La session a été mise en pause par le serveur"
    ];

    private readonly E2ETestHostFixture _hostFixture;
    private readonly PlaywrightFixture _playwrightFixture;

    public ReconnectModalE2ETests(E2ETestHostFixture hostFixture, PlaywrightFixture playwrightFixture)
    {
        _hostFixture = hostFixture;
        _playwrightFixture = playwrightFixture;
    }

    [Fact]
    public async Task ReconnectModal_ne_devrait_pas_exposer_de_textes_en_etat_nominal_et_devrait_rester_declenchable()
    {
        await using var browser = await _playwrightFixture.Playwright.Chromium.LaunchAsync(_playwrightFixture.CreateLaunchOptions());
        var page = await browser.NewPageAsync(new BrowserNewPageOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = 1600,
                Height = 1000
            }
        });

        await page.GotoAsync(new Uri(_hostFixture.AppBaseAddress, "/").ToString());
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Starter Blazor/.NET pour application métier" })).ToBeVisibleAsync();

        var texteInitial = await LireTexteRenduAsync(page);
        AssertNeContientAucunTexteParasite(texteInitial);

        var modal = page.Locator("#bes-reconnect-modal");
        await Expect(modal).ToBeAttachedAsync();
        Assert.Equal(string.Empty, await modal.InnerHTMLAsync());

        await page.EvaluateAsync(
            """
            () => {
                const modal = document.getElementById("bes-reconnect-modal");

                if (!modal) {
                    throw new Error("Le modal de reconnexion personnalisé est introuvable.");
                }

                modal.dispatchEvent(new CustomEvent("components-reconnect-state-changed", {
                    detail: { state: "show" }
                }));
            }
            """);

        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Connexion interrompue" })).ToBeVisibleAsync();
        await Expect(page.GetByText("Tentative de reconnexion au serveur en cours.")).ToBeVisibleAsync();

        await page.EvaluateAsync(
            """
            () => {
                const modal = document.getElementById("bes-reconnect-modal");

                if (!modal) {
                    throw new Error("Le modal de reconnexion personnalisé est introuvable.");
                }

                modal.dispatchEvent(new CustomEvent("components-reconnect-state-changed", {
                    detail: { state: "hide" }
                }));
            }
            """);

        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Connexion interrompue" })).ToHaveCountAsync(0);
        await Expect(page.GetByText("Tentative de reconnexion au serveur en cours.")).ToHaveCountAsync(0);
        Assert.Equal(string.Empty, await modal.InnerHTMLAsync());

        var texteFinal = await LireTexteRenduAsync(page);
        AssertNeContientAucunTexteParasite(texteFinal);
    }

    private static async Task<string> LireTexteRenduAsync(IPage page) =>
        await page.EvaluateAsync<string>(
            """
            () => document.body.textContent ?? ""
            """);

    private static void AssertNeContientAucunTexteParasite(string texte)
    {
        foreach (var texteParasite in TextesParasitesAttendus)
        {
            Assert.DoesNotContain(texteParasite, texte, StringComparison.Ordinal);
        }
    }
}
