namespace BlazorEnterpriseStarter.Shared.Contracts.Backlog;

/// <summary>
/// Représente un élément de backlog transporté entre le front-end et l’API.
/// </summary>
public sealed record BacklogItemDto(
    Guid Id,
    string Titre,
    string Description,
    BacklogItemStatus Statut,
    BacklogItemPriority Priorite,
    DateTimeOffset DateCreation);
