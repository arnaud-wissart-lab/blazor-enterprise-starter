using Bunit;
using BlazorEnterpriseStarter.App.State.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using BlazorEnterpriseStarter.Tests.Backlog.Fakes;
using Microsoft.Extensions.DependencyInjection;
using BacklogPage = BlazorEnterpriseStarter.App.Components.Pages.Backlog;

namespace BlazorEnterpriseStarter.Tests.Ui.Pages;

public sealed class BacklogPageTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void Backlog_devrait_afficher_l_empty_state_quand_aucun_element_n_est_retourne()
    {
        var apiClient = new FakeBacklogApiClient
        {
            ListResult = new PagedResultDto<BacklogItemDto>([], 0, 1, 6)
        };

        var component = RenderBacklog(apiClient);

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Aucun élément ne correspond aux critères", component.Markup);
            Assert.Contains("Créer un élément", component.Markup);
        });
    }

    [Fact]
    public void Backlog_devrait_ouvrir_la_modale_de_creation_depuis_l_action_principale()
    {
        var apiClient = new FakeBacklogApiClient();
        var component = RenderBacklog(apiClient);

        component.WaitForAssertion(() => Assert.Contains("Créer un élément", component.Markup));

        component.FindAll("button")
            .Single(button => button.TextContent.Contains("Créer un élément", StringComparison.Ordinal))
            .Click();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Créer un élément de backlog", component.Markup);
            Assert.Contains("Fiche backlog", component.Markup);
            Assert.Contains("Nouveau", component.Markup);
            Assert.Contains("Moyenne", component.Markup);
        });
    }

    [Fact]
    public void Backlog_devrait_appliquer_une_vue_rapide_et_afficher_le_filtre_actif()
    {
        var apiClient = new FakeBacklogApiClient();
        var component = RenderBacklog(apiClient);

        component.WaitForAssertion(() => Assert.Equal(1, apiClient.NombreAppelsListe));

        component.FindAll("button")
            .Single(button => button.TextContent.Contains("Prêts à lancer", StringComparison.Ordinal))
            .Click();

        component.WaitForAssertion(() =>
        {
            Assert.Equal(BacklogItemStatus.Pret, apiClient.DerniereRequete?.Statut);
            Assert.Contains("Statut : Prêt", component.Markup);
        });
    }

    private IRenderedComponent<BacklogPage> RenderBacklog(FakeBacklogApiClient apiClient)
    {
        _context.Services.AddSingleton(apiClient);
        _context.Services.AddScoped<BacklogState>(provider => new BacklogState(provider.GetRequiredService<FakeBacklogApiClient>()));

        return _context.Render<BacklogPage>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
