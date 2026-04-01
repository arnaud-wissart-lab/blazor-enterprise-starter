namespace BlazorEnterpriseStarter.Shared.Contracts.Backlog;

/// <summary>
/// Représente les principaux statuts d’un élément de backlog produit.
/// </summary>
public enum BacklogItemStatus
{
    Nouveau,
    Pret,
    EnCours,
    Termine,
    Archive
}
