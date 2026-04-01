using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.App.State.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using BlazorEnterpriseStarter.Tests.Backlog.Fakes;

namespace BlazorEnterpriseStarter.Tests.Backlog;

public class BacklogStateTests
{
    [Fact]
    public async Task InitializeAsync_devrait_charger_les_elements_et_marquer_le_succes()
    {
        var apiClient = new FakeBacklogApiClient();
        var state = new BacklogState(apiClient);
        var notifications = 0;

        state.Changed += () => notifications++;

        await state.InitializeAsync(CancellationToken.None);

        Assert.True(state.HasLoadedOnce);
        Assert.Equal(BacklogRequestStatus.Success, state.ListStatus);
        Assert.Equal(2, state.TotalCount);
        Assert.True(notifications >= 2);
    }

    [Fact]
    public async Task InitializeAsync_ne_devrait_pas_recharger_si_un_resultat_est_deja_disponible()
    {
        var apiClient = new FakeBacklogApiClient();
        var state = new BacklogState(apiClient);

        await state.InitializeAsync(CancellationToken.None);
        await state.InitializeAsync(CancellationToken.None);

        Assert.Equal(1, apiClient.NombreAppelsListe);
    }

    [Fact]
    public async Task ApplyFiltersAsync_devrait_mettre_a_jour_la_requete_et_recharger()
    {
        var apiClient = new FakeBacklogApiClient();
        var state = new BacklogState(apiClient);

        await state.ApplyFiltersAsync(
            new BacklogItemsQueryDto
            {
                Recherche = "tableau",
                Statut = BacklogItemStatus.EnCours,
                Priorite = BacklogItemPriority.Haute,
                Tri = BacklogItemSortField.Priorite,
                Direction = DirectionTri.Croissante,
                NumeroPage = 3,
                TaillePage = 12
            },
            CancellationToken.None);

        Assert.Equal("tableau", state.Query.Recherche);
        Assert.Equal(BacklogItemStatus.EnCours, state.Query.Statut);
        Assert.Equal(BacklogItemPriority.Haute, state.Query.Priorite);
        Assert.Equal(BacklogItemSortField.Priorite, state.Query.Tri);
        Assert.Equal(DirectionTri.Croissante, state.Query.Direction);
        Assert.Equal(1, state.Query.NumeroPage);
        Assert.Equal(12, state.Query.TaillePage);
        Assert.Equal("tableau", apiClient.DerniereRequete?.Recherche);
    }

    [Fact]
    public async Task CreateAsync_en_cas_derreur_devrait_exposer_les_informations_de_validation()
    {
        var apiClient = new FakeBacklogApiClient
        {
            ExceptionCreation = new BacklogApiException(
                "La validation a échoué.",
                new Dictionary<string, string[]>
                {
                    [nameof(BacklogItemUpsertRequest.Titre)] = ["Le titre est obligatoire."]
                })
        };
        var state = new BacklogState(apiClient);

        await Assert.ThrowsAsync<BacklogApiException>(() =>
            state.CreateAsync(
                new BacklogItemUpsertRequest
                {
                    Titre = string.Empty,
                    Description = "Description invalide pour le test."
                },
                CancellationToken.None));

        Assert.Equal(BacklogRequestStatus.Error, state.MutationStatus);
        Assert.Equal("La validation a échoué.", state.MutationErrorMessage);
        Assert.True(state.MutationErrors.ContainsKey(nameof(BacklogItemUpsertRequest.Titre)));
    }

    [Fact]
    public async Task ResetMutationFeedback_devrait_effacer_les_messages_et_revenir_a_idle()
    {
        var apiClient = new FakeBacklogApiClient
        {
            ExceptionCreation = new BacklogApiException(
                "La validation a échoué.",
                new Dictionary<string, string[]>
                {
                    [nameof(BacklogItemUpsertRequest.Titre)] = ["Le titre est obligatoire."]
                })
        };
        var state = new BacklogState(apiClient);

        await Assert.ThrowsAsync<BacklogApiException>(() =>
            state.CreateAsync(
                new BacklogItemUpsertRequest
                {
                    Titre = string.Empty,
                    Description = "Description invalide pour le test."
                },
                CancellationToken.None));

        state.ResetMutationFeedback();

        Assert.Equal(BacklogRequestStatus.Idle, state.MutationStatus);
        Assert.Null(state.MutationErrorMessage);
        Assert.Empty(state.MutationErrors);
    }

    [Fact]
    public async Task RefreshAsync_en_cas_derreur_apres_un_succes_devrait_conserver_les_donnees_existantes()
    {
        var apiClient = new FakeBacklogApiClient();
        var state = new BacklogState(apiClient);

        await state.InitializeAsync(CancellationToken.None);

        apiClient.ExceptionListe = new BacklogApiException("Le backend est momentanément indisponible.");

        await state.RefreshAsync(CancellationToken.None);

        Assert.Equal(BacklogRequestStatus.Error, state.ListStatus);
        Assert.Equal("Le backend est momentanément indisponible.", state.ListErrorMessage);
        Assert.Equal(2, state.Items.Count);
        Assert.True(state.HasResult);
    }

    [Fact]
    public async Task RefreshAsync_annule_apres_un_succes_ne_devrait_pas_laisser_l_etat_en_chargement()
    {
        var apiClient = new FakeBacklogApiClient();
        var state = new BacklogState(apiClient);

        await state.InitializeAsync(CancellationToken.None);

        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await state.RefreshAsync(cancellation.Token);

        Assert.Equal(BacklogRequestStatus.Success, state.ListStatus);
        Assert.Equal(2, state.Items.Count);
        Assert.Null(state.ListErrorMessage);
    }
}
