using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Application.Backlog;

/// <summary>
/// Centralise les règles de validation simples du module backlog.
/// </summary>
public static class BacklogValidation
{
    public static Dictionary<string, string[]>? ValiderRequete(BacklogItemsQueryDto requete)
    {
        var erreurs = new Dictionary<string, string[]>();

        if (requete.NumeroPage < 1)
        {
            erreurs[nameof(requete.NumeroPage)] = ["Le numéro de page doit être supérieur ou égal à 1."];
        }

        if (requete.TaillePage is < 1 or > 50)
        {
            erreurs[nameof(requete.TaillePage)] = ["La taille de page doit être comprise entre 1 et 50 éléments."];
        }

        if (!string.IsNullOrWhiteSpace(requete.Recherche) && requete.Recherche.Trim().Length > 120)
        {
            erreurs[nameof(requete.Recherche)] = ["La recherche ne peut pas dépasser 120 caractères."];
        }

        return erreurs.Count == 0 ? null : erreurs;
    }

    public static Dictionary<string, string[]>? ValiderCommande(BacklogItemUpsertRequest commande)
    {
        var erreurs = new Dictionary<string, string[]>();
        var titre = commande.Titre.Trim();
        var description = commande.Description.Trim();

        if (string.IsNullOrWhiteSpace(titre))
        {
            erreurs[nameof(commande.Titre)] = ["Le titre est obligatoire."];
        }
        else if (titre.Length > 120)
        {
            erreurs[nameof(commande.Titre)] = ["Le titre ne peut pas dépasser 120 caractères."];
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            erreurs[nameof(commande.Description)] = ["La description est obligatoire."];
        }
        else if (description.Length > 2000)
        {
            erreurs[nameof(commande.Description)] = ["La description ne peut pas dépasser 2 000 caractères."];
        }

        return erreurs.Count == 0 ? null : erreurs;
    }
}
