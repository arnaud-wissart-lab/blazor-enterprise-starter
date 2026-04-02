using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Persistence;

/// <summary>
/// Fournit un jeu de données initial stable pour la première création de la base.
/// </summary>
public static class BacklogSeedData
{
    private static readonly DateTimeOffset BaseDate = new(2026, 03, 20, 09, 00, 00, TimeSpan.Zero);

    public static IReadOnlyList<BacklogItemEntity> CreateInitialItems() =>
    [
        Create(
            "Créer le tableau de bord produit",
            "Assembler une première vue synthétique des indicateurs clés pour l’équipe produit.",
            BacklogItemStatus.EnCours,
            BacklogItemPriority.Critique,
            -10),
        Create(
            "Préparer la recherche transverse",
            "Définir un comportement unifié de recherche pour les listes métier principales.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Haute,
            -8),
        Create(
            "Définir les règles de priorisation",
            "Clarifier l’impact métier, l’urgence et la valeur attendue pour homogénéiser l’ordre du backlog.",
            BacklogItemStatus.Termine,
            BacklogItemPriority.Moyenne,
            -18),
        Create(
            "Ajouter l’historique des changements",
            "Permettre aux équipes de relire les évolutions d’un ticket directement depuis sa fiche.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Haute,
            -4),
        Create(
            "Préparer la revue hebdomadaire",
            "Structurer les informations nécessaires pour arbitrer les éléments prêts en comité produit.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Moyenne,
            -6),
        Create(
            "Concevoir l’écran de détail backlog",
            "Prévoir une fiche élément avec historique, commentaires et navigation vers les éléments liés.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Moyenne,
            -3),
        Create(
            "Industrialiser les notifications produit",
            "Définir les notifications utiles pour les changements de statut et les arbitrages prioritaires.",
            BacklogItemStatus.EnCours,
            BacklogItemPriority.Haute,
            -12),
        Create(
            "Ajouter la vue calendrier des jalons",
            "Présenter les échéances backlog dans une vue calendrier exploitable en comité.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Basse,
            -15),
        Create(
            "Structurer les filtres enregistrés",
            "Permettre aux équipes de retrouver rapidement leurs filtres récurrents sur les vues produit.",
            BacklogItemStatus.Nouveau,
            BacklogItemPriority.Haute,
            -5),
        Create(
            "Préparer les exports CSV",
            "Rendre possible l’export des éléments filtrés pour les revues externes et les reporting rapides.",
            BacklogItemStatus.Pret,
            BacklogItemPriority.Moyenne,
            -7),
        Create(
            "Archiver l’ancien workflow d’import",
            "Retirer le flux historique devenu obsolète depuis la nouvelle expérience de saisie.",
            BacklogItemStatus.Archive,
            BacklogItemPriority.Basse,
            -30)
    ];

    private static BacklogItemEntity Create(
        string titre,
        string description,
        BacklogItemStatus statut,
        BacklogItemPriority priorite,
        int decalageJours) =>
        new()
        {
            Id = Guid.NewGuid(),
            Titre = titre,
            Description = description,
            Statut = statut,
            Priorite = priorite,
            DateCreation = BaseDate.AddDays(decalageJours)
        };
}
