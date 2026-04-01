using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Domain.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Backlog;

/// <summary>
/// Fournit une persistence mémoire simple et immédiatement démontrable pour le backlog.
/// </summary>
public sealed class InMemoryBacklogRepository : IBacklogRepository
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, BacklogItem> _items;

    public InMemoryBacklogRepository()
    {
        var now = DateTimeOffset.UtcNow;

        _items = CreerJeuInitial(now)
            .ToDictionary(item => item.Id);
    }

    public Task<IReadOnlyList<BacklogItem>> ListerAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<BacklogItem>>(_items.Values.ToArray());
        }
    }

    public Task<BacklogItem?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            _items.TryGetValue(id, out var item);
            return Task.FromResult(item);
        }
    }

    public Task AjouterAsync(BacklogItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            _items[item.Id] = item;
        }

        return Task.CompletedTask;
    }

    public Task MettreAJourAsync(BacklogItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            _items[item.Id] = item;
        }

        return Task.CompletedTask;
    }

    public Task<bool> SupprimerAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            return Task.FromResult(_items.Remove(id));
        }
    }

    private static IEnumerable<BacklogItem> CreerJeuInitial(DateTimeOffset now)
    {
        yield return BacklogItem.Create(
            "Créer le tableau de bord produit",
            "Assembler une première vue synthétique des indicateurs clés pour l’équipe produit.",
            BacklogItemStatus.EnCours,
            BacklogItemPriority.Critique,
            now.AddDays(-10));

        yield return BacklogItem.Create(
            "Préparer la recherche transverse",
            "Définir un comportement unifié de recherche pour les listes métier principales.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Haute,
            now.AddDays(-8));

        yield return BacklogItem.Create(
            "Documenter les conventions UI",
            "Clarifier les règles de composition entre design system et composants applicatifs.",
            BacklogItemStatus.Termine,
            BacklogItemPriority.Moyenne,
            now.AddDays(-18));

        yield return BacklogItem.Create(
            "Ajouter la vue backlog produit",
            "Mettre à disposition une page métier démonstrative avec recherche, filtre et CRUD.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Haute,
            now.AddDays(-4));

        yield return BacklogItem.Create(
            "Préparer les scénarios de démonstration",
            "Lister les parcours à montrer pour la vitrine GitHub et les démonstrations locales.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Moyenne,
            now.AddDays(-6));

        yield return BacklogItem.Create(
            "Archiver l’ancien prototype de navigation",
            "Conserver la trace de l’itération précédente avant la refonte du layout principal.",
            BacklogItemStatus.Archive,
            BacklogItemPriority.Basse,
            now.AddDays(-30));
    }
}
