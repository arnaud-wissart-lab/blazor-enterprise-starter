using BlazorEnterpriseStarter.Server.Domain.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Application.Backlog;

/// <summary>
/// Implémente les cas d’usage du backlog produit au-dessus d’un dépôt abstrait.
/// </summary>
public sealed class BacklogService(IBacklogRepository repository) : IBacklogService
{
    public async Task<PagedResultDto<BacklogItemDto>> ListerAsync(BacklogItemsQueryDto requete, CancellationToken cancellationToken)
    {
        var items = await repository.ListerAsync(cancellationToken);
        var recherche = requete.Recherche?.Trim();

        IEnumerable<BacklogItem> sequence = items;

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            sequence = sequence.Where(item =>
                item.Titre.Contains(recherche, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(recherche, StringComparison.OrdinalIgnoreCase));
        }

        if (requete.Statut is not null)
        {
            sequence = sequence.Where(item => item.Statut == requete.Statut);
        }

        if (requete.Priorite is not null)
        {
            sequence = sequence.Where(item => item.Priorite == requete.Priorite);
        }

        sequence = AppliquerTri(sequence, requete);

        var nombreTotal = sequence.Count();
        var elements = sequence
            .Skip((requete.NumeroPage - 1) * requete.TaillePage)
            .Take(requete.TaillePage)
            .Select(MapperVersDto)
            .ToArray();

        return new PagedResultDto<BacklogItemDto>(
            elements,
            nombreTotal,
            requete.NumeroPage,
            requete.TaillePage);
    }

    public async Task<BacklogItemDto?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await repository.ObtenirParIdAsync(id, cancellationToken);
        return item is null ? null : MapperVersDto(item);
    }

    public async Task<BacklogItemDto> CreerAsync(BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
    {
        var item = BacklogItem.Create(
            commande.Titre.Trim(),
            commande.Description.Trim(),
            commande.Statut,
            commande.Priorite,
            DateTimeOffset.UtcNow);

        await repository.AjouterAsync(item, cancellationToken);
        return MapperVersDto(item);
    }

    public async Task<BacklogItemDto?> ModifierAsync(Guid id, BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
    {
        var item = await repository.ObtenirParIdAsync(id, cancellationToken);

        if (item is null)
        {
            return null;
        }

        var itemMisAJour = item.MettreAJour(
            commande.Titre.Trim(),
            commande.Description.Trim(),
            commande.Statut,
            commande.Priorite);

        await repository.MettreAJourAsync(itemMisAJour, cancellationToken);
        return MapperVersDto(itemMisAJour);
    }

    public Task<bool> SupprimerAsync(Guid id, CancellationToken cancellationToken) =>
        repository.SupprimerAsync(id, cancellationToken);

    private static IEnumerable<BacklogItem> AppliquerTri(IEnumerable<BacklogItem> sequence, BacklogItemsQueryDto requete)
    {
        var sensDecroissant = requete.Direction == DirectionTri.Decroissante;

        return requete.Tri switch
        {
            BacklogItemSortField.Priorite => sensDecroissant
                ? sequence.OrderByDescending(item => item.Priorite).ThenByDescending(item => item.DateCreation)
                : sequence.OrderBy(item => item.Priorite).ThenBy(item => item.DateCreation),
            BacklogItemSortField.Statut => sensDecroissant
                ? sequence.OrderByDescending(item => item.Statut).ThenByDescending(item => item.DateCreation)
                : sequence.OrderBy(item => item.Statut).ThenBy(item => item.DateCreation),
            BacklogItemSortField.Titre => sensDecroissant
                ? sequence.OrderByDescending(item => item.Titre).ThenByDescending(item => item.DateCreation)
                : sequence.OrderBy(item => item.Titre).ThenBy(item => item.DateCreation),
            _ => sensDecroissant
                ? sequence.OrderByDescending(item => item.DateCreation)
                : sequence.OrderBy(item => item.DateCreation)
        };
    }

    private static BacklogItemDto MapperVersDto(BacklogItem item) =>
        new(
            item.Id,
            item.Titre,
            item.Description,
            item.Statut,
            item.Priorite,
            item.DateCreation);
}
