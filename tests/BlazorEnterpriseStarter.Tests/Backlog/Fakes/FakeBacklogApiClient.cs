using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Tests.Backlog.Fakes;

internal sealed class FakeBacklogApiClient : IBacklogApiClient
{
    private PagedResultDto<BacklogItemDto> _listResult = CreerResultatParDefaut();

    public int NombreAppelsListe { get; private set; }

    public BacklogItemsQueryDto? DerniereRequete { get; private set; }

    public BacklogApiException? ExceptionListe { get; set; }

    public BacklogApiException? ExceptionCreation { get; init; }

    public PagedResultDto<BacklogItemDto> ListResult
    {
        get => _listResult;
        set => _listResult = value;
    }

    public Task<PagedResultDto<BacklogItemDto>> ListerAsync(BacklogItemsQueryDto requete, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        NombreAppelsListe++;
        DerniereRequete = CopierRequete(requete);

        if (ExceptionListe is not null)
        {
            throw ExceptionListe;
        }

        return Task.FromResult(_listResult);
    }

    public Task<BacklogItemDto> CreerAsync(BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

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

    public Task<BacklogItemDto> ModifierAsync(Guid id, BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(
            new BacklogItemDto(
                id,
                commande.Titre,
                commande.Description,
                commande.Statut,
                commande.Priorite,
                DateTimeOffset.UtcNow));
    }

    public Task SupprimerAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    private static BacklogItemsQueryDto CopierRequete(BacklogItemsQueryDto requete) =>
        new()
        {
            Recherche = requete.Recherche,
            Statut = requete.Statut,
            Priorite = requete.Priorite,
            Tri = requete.Tri,
            Direction = requete.Direction,
            NumeroPage = requete.NumeroPage,
            TaillePage = requete.TaillePage
        };

    private static PagedResultDto<BacklogItemDto> CreerResultatParDefaut() =>
        new(
            [
                new BacklogItemDto(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    "Créer le tableau de bord",
                    "Préparer la première vue produit.",
                    BacklogItemStatus.EnCours,
                    BacklogItemPriority.Haute,
                    DateTimeOffset.UtcNow.AddDays(-3)),
                new BacklogItemDto(
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    "Structurer la feuille de route",
                    "Définir les prochains jalons métier.",
                    BacklogItemStatus.Pret,
                    BacklogItemPriority.Moyenne,
                    DateTimeOffset.UtcNow.AddDays(-8))
            ],
            2,
            1,
            6);
}
