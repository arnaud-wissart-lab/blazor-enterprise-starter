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
            "Définir les règles de priorisation",
            "Clarifier l’impact métier, l’urgence et la valeur attendue pour homogénéiser l’ordre du backlog.",
            BacklogItemStatus.Termine,
            BacklogItemPriority.Moyenne,
            now.AddDays(-18));

        yield return BacklogItem.Create(
            "Ajouter l’historique des changements",
            "Permettre aux équipes de relire les évolutions d’un ticket directement depuis sa fiche.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Haute,
            now.AddDays(-4));

        yield return BacklogItem.Create(
            "Préparer la revue hebdomadaire",
            "Structurer les informations nécessaires pour arbitrer les éléments prêts en comité produit.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Moyenne,
            now.AddDays(-6));

        yield return BacklogItem.Create(
            "Concevoir l’écran de détail backlog",
            "Prévoir une fiche élément avec historique, commentaires et navigation vers les éléments liés.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Moyenne,
            now.AddDays(-3));

        yield return BacklogItem.Create(
            "Industrialiser les notifications produit",
            "Définir les notifications utiles pour les changements de statut et les arbitrages prioritaires.",
            BacklogItemStatus.EnCours,
            BacklogItemPriority.Haute,
            now.AddDays(-12));

        yield return BacklogItem.Create(
            "Ajouter la vue calendrier des jalons",
            "Présenter les échéances backlog dans une vue calendrier exploitable en comité.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Basse,
            now.AddDays(-15));

        yield return BacklogItem.Create(
            "Structurer les filtres enregistrés",
            "Permettre aux équipes de retrouver rapidement leurs filtres récurrents sur les vues produit.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Haute,
            now.AddDays(-5));

        yield return BacklogItem.Create(
            "Préparer les exports CSV",
            "Rendre possible l’export des éléments filtrés pour les revues externes et les reporting rapides.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Moyenne,
            now.AddDays(-7));

        yield return BacklogItem.Create(
            "Archiver l’ancien workflow d’import",
            "Retirer le flux historique devenu obsolète depuis la nouvelle expérience de saisie.",
            BacklogItemStatus.Archive,
            BacklogItemPriority.Basse,
            now.AddDays(-30));
    }
}
