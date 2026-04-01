namespace BlazorEnterpriseStarter.Shared.Contracts.Backlog;

/// <summary>
/// Représente la commande de création ou de modification d’un élément de backlog.
/// </summary>
public sealed record BacklogItemUpsertRequest
{
    public string Titre { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public BacklogItemStatus Statut { get; init; } = BacklogItemStatus.Nouveau;

    public BacklogItemPriority Priorite { get; init; } = BacklogItemPriority.Moyenne;
}
