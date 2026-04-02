using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Persistence;

/// <summary>
/// Représente l’enregistrement persisté d’un élément de backlog dans SQLite.
/// </summary>
public sealed class BacklogItemEntity
{
    public Guid Id { get; set; }

    public string Titre { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public BacklogItemStatus Statut { get; set; }

    public BacklogItemPriority Priorite { get; set; }

    public DateTimeOffset DateCreation { get; set; }
}
