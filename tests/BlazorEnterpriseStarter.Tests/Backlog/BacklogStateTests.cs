using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.App.State.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

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

    private sealed class FakeBacklogApiClient : IBacklogApiClient
    {
        private readonly PagedResultDto<BacklogItemDto> _result =
            new(
                [
                    new BacklogItemDto(
                        Guid.NewGuid(),
                        "Créer le tableau de bord",
                        "Préparer la première vue produit.",
                        BacklogItemStatus.EnCours,
                        BacklogItemPriority.Haute,
                        DateTimeOffset.UtcNow.AddDays(-3)),
                    new BacklogItemDto(
                        Guid.NewGuid(),
                        "Structurer la feuille de route",
                        "Définir les prochains jalons métier.",
                        BacklogItemStatus.Pret,
                        BacklogItemPriority.Moyenne,
                        DateTimeOffset.UtcNow.AddDays(-8))
                ],
                2,
                1,
                6);

        public BacklogItemsQueryDto? DerniereRequete { get; private set; }

        public BacklogApiException? ExceptionCreation { get; init; }

        public Task<PagedResultDto<BacklogItemDto>> ListerAsync(BacklogItemsQueryDto requete, CancellationToken cancellationToken)
        {
            DerniereRequete = requete;
            return Task.FromResult(_result);
        }

        public Task<BacklogItemDto> CreerAsync(BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
        {
            if (ExceptionCreation is not null)
            {
                throw ExceptionCreation;
            }

            return Task.FromResult(
                new BacklogItemDto(
                    Guid.NewGuid(),
                    commande.Titre,
                    commande.Description,
                    commande.Statut,
                    commande.Priorite,
                    DateTimeOffset.UtcNow));
        }

        public Task<BacklogItemDto> ModifierAsync(Guid id, BacklogItemUpsertRequest commande, CancellationToken cancellationToken) =>
            Task.FromResult(
                new BacklogItemDto(
                    id,
                    commande.Titre,
                    commande.Description,
                    commande.Statut,
                    commande.Priorite,
                    DateTimeOffset.UtcNow));

        public Task SupprimerAsync(Guid id, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
