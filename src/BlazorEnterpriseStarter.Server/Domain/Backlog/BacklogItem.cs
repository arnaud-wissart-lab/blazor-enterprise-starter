using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Domain.Backlog;

/// <summary>
/// Représente l’entité métier minimale d’un élément de backlog produit.
/// </summary>
public sealed record BacklogItem(
    Guid Id,
    string Titre,
    string Description,
    BacklogItemStatus Statut,
    BacklogItemPriority Priorite,
    DateTimeOffset DateCreation)
{
    public static BacklogItem Create(
        string titre,
        string description,
        BacklogItemStatus statut,
        BacklogItemPriority priorite,
        DateTimeOffset dateCreation) =>
        new(Guid.NewGuid(), titre, description, statut, priorite, dateCreation);

    public BacklogItem MettreAJour(
        string titre,
        string description,
        BacklogItemStatus statut,
        BacklogItemPriority priorite) =>
        this with
        {
            Titre = titre,
            Description = description,
            Statut = statut,
            Priorite = priorite
        };
}
