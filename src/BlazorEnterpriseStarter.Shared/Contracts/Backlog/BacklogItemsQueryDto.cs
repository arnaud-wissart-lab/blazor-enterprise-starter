namespace BlazorEnterpriseStarter.Shared.Contracts.Backlog;

/// <summary>
/// Représente les paramètres de recherche, de filtre et de pagination d’une liste backlog.
/// </summary>
public sealed class BacklogItemsQueryDto
{
    public string? Recherche { get; set; }

    public BacklogItemStatus? Statut { get; set; }

    public BacklogItemPriority? Priorite { get; set; }

    public BacklogItemSortField Tri { get; set; } = BacklogItemSortField.DateCreation;

    public DirectionTri Direction { get; set; } = DirectionTri.Decroissante;

    public int NumeroPage { get; set; } = 1;

    public int TaillePage { get; set; } = 6;
}
